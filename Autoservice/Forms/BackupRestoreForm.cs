using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autoservice.Data;
using Autoservice.Services;
using Newtonsoft.Json;

namespace Autoservice.Forms
{
    public partial class BackupRestoreForm : Form
    {
        private BackupService backupService;
        private AutoServiceContext context;

        public BackupRestoreForm()
        {
            InitializeComponent();
            this.Text = "Резервная копия и архив";
            this.Size = new System.Drawing.Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            backupService = new BackupService(
                "localhost,1435",      
                "AutoServiceDB",       
                "sa",                  
                "Strpo!2025passA1"     
                           
);
            context = new AutoServiceContext();
        }

        private void BackupRestoreForm_Load(object sender, EventArgs e)
        {
            CreateUI();
        }

        private void CreateUI()
        {
            // backup
            GroupBox gbBackup = new GroupBox { Text = "Резервная копия", Left = 10, Top = 10, Width = 560, Height = 100 };
            Label lblBackupPath = new Label { Text = "Путь для сохранения:", Left = 10, Top = 20, Width = 150 };
            TextBox txtBackupPath = new TextBox { Left = 170, Top = 20, Width = 300 };
            Button btnBrowseBackup = new Button { Text = "Обзор...", Left = 480, Top = 20, Width = 60 };
            Button btnCreateBackup = new Button { Text = "Создать резервную копию", Left = 10, Top = 60, Width = 200, Height = 30 };

            btnBrowseBackup.Click += (s, e) =>
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "BAK files (*.bak)|*.bak",
                    DefaultExt = "bak"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                    txtBackupPath.Text = sfd.FileName;
            };

            btnCreateBackup.Click += (s, e) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(txtBackupPath.Text))
                    {
                        MessageBox.Show("Укажи путь для сохранения");
                        return;
                    }

                    backupService.CreateBackup(txtBackupPath.Text);
                    MessageBox.Show("Резервная копия создана успешно!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            };

            gbBackup.Controls.Add(lblBackupPath);
            gbBackup.Controls.Add(txtBackupPath);
            gbBackup.Controls.Add(btnBrowseBackup);
            gbBackup.Controls.Add(btnCreateBackup);

            // restore
            GroupBox gbRestore = new GroupBox { Text = "Восстановление", Left = 10, Top = 120, Width = 560, Height = 100 };
            Label lblRestorePath = new Label { Text = "Путь к файлу бэкапа:", Left = 10, Top = 20, Width = 150 };
            TextBox txtRestorePath = new TextBox { Left = 170, Top = 20, Width = 300 };
            Button btnBrowseRestore = new Button { Text = "Обзор...", Left = 480, Top = 20, Width = 60 };
            Button btnRestore = new Button { Text = "Восстановить из копии", Left = 10, Top = 60, Width = 200, Height = 30 };

            btnBrowseRestore.Click += (s, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "BAK files (*.bak)|*.bak"
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtRestorePath.Text = ofd.FileName;
            };

            btnRestore.Click += (s, e) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(txtRestorePath.Text))
                    {
                        MessageBox.Show("Укажи путь к файлу бэкапа");
                        return;
                    }

                    if (MessageBox.Show("Это перезапишет текущую БД! Продолжить?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        backupService.RestoreBackup(txtRestorePath.Text);
                        MessageBox.Show("БД восстановлена успешно!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            };

            gbRestore.Controls.Add(lblRestorePath);
            gbRestore.Controls.Add(txtRestorePath);
            gbRestore.Controls.Add(btnBrowseRestore);
            gbRestore.Controls.Add(btnRestore);

            // архивирование
            GroupBox gbArchive = new GroupBox { Text = "Архивирование таблиц", Left = 10, Top = 230, Width = 560, Height = 120 };
            Label lblTable = new Label { Text = "Таблица:", Left = 10, Top = 20, Width = 100 };
            ComboBox cmbTable = new ComboBox { Left = 110, Top = 17, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTable.Items.AddRange(new[] { "Заказы", "Клиенты", "Автомобили", "Сотрудники", "Запчасти", "Услуги" });
            cmbTable.SelectedIndex = 0;

            Button btnArchive = new Button { Text = "Архивировать и удалить", Left = 10, Top = 60, Width = 200, Height = 30 };
            Button btnViewArchives = new Button { Text = "Просмотр архивов", Left = 220, Top = 60, Width = 150, Height = 30 };

            btnArchive.Click += (s, e) => ArchiveTable(cmbTable.SelectedItem.ToString());
            btnViewArchives.Click += (s, e) => ViewArchives();

            gbArchive.Controls.Add(lblTable);
            gbArchive.Controls.Add(cmbTable);
            gbArchive.Controls.Add(btnArchive);
            gbArchive.Controls.Add(btnViewArchives);

            // Кнопка закрытия
            Button btnClose = new Button { Text = "Закрыть", Left = 480, Top = 360, Width = 80, Height = 30 };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(gbBackup);
            this.Controls.Add(gbRestore);
            this.Controls.Add(gbArchive);
            this.Controls.Add(btnClose);
        }

        private void ArchiveTable(string tableName)
        {
            var confirmResult = MessageBox.Show(
                $"Архивировать и удалить все данные из '{tableName}'?",
                "Подтверждение",
                MessageBoxButtons.YesNo);

            if (confirmResult != DialogResult.Yes) return;

            try
            {
                var sfd = new SaveFileDialog
                {
                    Filter = "Архив (*.json)|*.json",
                    DefaultExt = "json",
                    FileName = $"archive_{tableName}_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (sfd.ShowDialog() != DialogResult.OK) return;

                var data = GetTableData(tableName);

                if (data.Count == 0)
                {
                    MessageBox.Show("Таблица пуста");
                    return;
                }

                string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(sfd.FileName, json, Encoding.UTF8);

                DeleteTableData(tableName);

                MessageBox.Show($"Архив сохранен: {Path.GetFileName(sfd.FileName)}\nДанные удалены из БД");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private List<object> GetTableData(string tableName)
        {
            switch (tableName)
            {
                case "Заказы":
                    return context.Orders.Include(o => o.Car).Include(o => o.Employee)
                        .ToList()
                        .Select(o => new { o.OrderID, o.Status, o.TotalAmount, o.DateReceived })
                        .ToList<object>();

                case "Клиенты":
                    return context.Clients
                        .Select(c => new { c.ClientID, c.FullName, c.Phone, c.Email, c.ClientType })
                        .ToList<object>();

                case "Автомобили":
                    return context.Cars.Include(c => c.Client)
                        .Select(c => new { c.CarID, c.Brand, c.Model, c.Year, c.VIN })
                        .ToList<object>();

                case "Сотрудники":
                    return context.Employees
                        .Select(e => new { e.EmployeeID, e.FullName, e.Position, e.Phone, e.Salary })
                        .ToList<object>();

                case "Запчасти":
                    return context.Parts
                        .Select(p => new { p.PartID, p.PartName, p.PartNumber, p.Manufacturer, p.Price })
                        .ToList<object>();

                case "Услуги":
                    return context.Services
                        .Select(s => new { s.ServiceID, s.ServiceName, s.Description, s.Price })
                        .ToList<object>();

                default:
                    return new List<object>();
            }
        }

        private void DeleteTableData(string tableName)
        {
            switch (tableName)
            {
                case "Заказы":
                    context.Orders.RemoveRange(context.Orders);
                    break;
                case "Клиенты":
                    context.Clients.RemoveRange(context.Clients);
                    break;
                case "Автомобили":
                    context.Cars.RemoveRange(context.Cars);
                    break;
                case "Сотрудники":
                    context.Employees.RemoveRange(context.Employees);
                    break;
                case "Запчасти":
                    context.Parts.RemoveRange(context.Parts);
                    break;
                case "Услуги":
                    context.Services.RemoveRange(context.Services);
                    break;
            }
            context.SaveChanges();
        }

        private void ViewArchives()
        {
            var ofd = new OpenFileDialog { Filter = "Archive files (*.json)|*.json" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string json = File.ReadAllText(ofd.FileName);
                    MessageBox.Show("Файл архива открыт. Содержимое:\n\n" + json.Substring(0, Math.Min(500, json.Length)) + "...");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void BackupRestoreForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            context?.Dispose();
        }
    }
}
