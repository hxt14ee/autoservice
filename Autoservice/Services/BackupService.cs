using System;
using System.Diagnostics;
using System.IO;

namespace Autoservice.Services
{
    public class BackupService
    {
        private readonly string serverName;
        private readonly string databaseName;
        private readonly string userName;
        private readonly string password;

        private readonly string containerName;


        private const string ContainerBackupDir = "/var/opt/mssql/backup";

        public BackupService(
            string serverName,
            string databaseName,
            string userName,
            string password,
            string containerName = "autoservice_mssql")
        {
            this.serverName = serverName;
            this.databaseName = databaseName;
            this.userName = userName;
            this.password = password;
            this.containerName = containerName;
        }


        public void CreateBackup(string windowsBackupPath)
        {
            if (string.IsNullOrWhiteSpace(windowsBackupPath))
                throw new ArgumentException("Не указан путь для сохранения бэкапа");

            string fileName = Path.GetFileName(windowsBackupPath);
            string containerBackupPath = $"{ContainerBackupDir}/{fileName}";


            string backupCommand =
                $"BACKUP DATABASE [{databaseName}] " +
                $"TO DISK = N'{containerBackupPath}' " +
                $"WITH INIT, NAME = N'Full Backup'";
            ExecuteSqlCommand(backupCommand);


            DockerCopyFromContainer(containerBackupPath, windowsBackupPath);
        }


        public void RestoreBackup(string windowsBackupPath)
        {
            if (!File.Exists(windowsBackupPath))
                throw new FileNotFoundException("Файл бэкапа не найден", windowsBackupPath);

            string fileName = Path.GetFileName(windowsBackupPath);
            string containerBackupPath = $"{ContainerBackupDir}/{fileName}";


            DockerCopyToContainer(windowsBackupPath, containerBackupPath);

            string restoreCommand =
                $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                $"RESTORE DATABASE [{databaseName}] " +
                $"FROM DISK = N'{containerBackupPath}' WITH REPLACE; " +
                $"ALTER DATABASE [{databaseName}] SET MULTI_USER;";

            ExecuteSqlCommand(restoreCommand);
        }

        private void ExecuteSqlCommand(string command)
        {
            string arguments =
                $"-S {serverName} -U {userName} -P {password} -Q \"{command}\"";

            RunProcess("sqlcmd", arguments, "sqlcmd");
        }

        private void DockerCopyFromContainer(string containerPath, string windowsPath)
        {
            string arguments =
                $"cp \"{containerName}:{containerPath}\" \"{windowsPath}\"";

            RunProcess("docker", arguments, "docker cp (из контейнера)");
        }

        private void DockerCopyToContainer(string windowsPath, string containerPath)
        {
            string arguments =
                $"cp \"{windowsPath}\" \"{containerName}:{containerPath}\"";

            RunProcess("docker", arguments, "docker cp (в контейнер)");
        }

        private void RunProcess(string fileName, string arguments, string actionName)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var process = Process.Start(psi);
            if (process == null)
                throw new Exception($"{actionName}: не удалось запустить процесс {fileName}.");

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception(
                    $"{actionName} завершилось с ошибкой (код {process.ExitCode}).\n" +
                    $"STDOUT: {stdout}\nSTDERR: {stderr}");
            }

            process.Dispose();
        }
    }
}
