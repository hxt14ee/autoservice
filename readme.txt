Сначала нужно поднять SQL Server в Docker. Файл docker\docker-compose.yml уже содержит нужную конфигурацию: контейнер с именем autoservice_mssql, порт хоста 1435, пользователь sa и пароль Strpo!2025passA1. Чтобы запустить контейнер, надо терминал в каталоге docker и выполнить команду:

docker compose up -d


После этого контейнер должен появиться в docker ps и слушать localhost:1435.

Дальше нужно восстановить базу через SQL Server Management Studio. Сначала резервную копию нужно положить внутрь контейнера. Файл db\AutoServiceDB.bak копируется в каталог /var/opt/mssql/backup так:

docker exec -it autoservice_mssql mkdir -p /var/opt/mssql/backup
docker cp db\AutoServiceDB.bak autoservice_mssql:/var/opt/mssql/backup/AutoServiceDB.bak


После этого подключаемся к серверу из SSMS: сервер localhost,1435, авторизация SQL Server, логин sa, пароль Strpo!2025passA1. Чтобы убедиться, что он читается и посмотреть логические имена файлов:

RESTORE FILELISTONLY
FROM DISK = N'/var/opt/mssql/backup/AutoServiceDB.bak';


Потом выполняется восстановление самой базы в стандартный каталог данных контейнера:

RESTORE DATABASE [AutoServiceDB]
FROM DISK = N'/var/opt/mssql/backup/AutoServiceDB.bak'
WITH MOVE 'AutoServiceDB'     TO '/var/opt/mssql/data/AutoServiceDB.mdf',
     MOVE 'AutoServiceDB_log' TO '/var/opt/mssql/data/AutoServiceDB_log.ldf',
     REPLACE, RECOVERY;


После этого база AutoServiceDB появляется в списке баз на сервере localhost,1435, и с ней уже работает приложение.

В App.config проекта строка подключения сразу настроена на этот контейнер.


Приложение можно либо собрать из исходников, открыв src\Autoservice.sln в Visual Studio и собрав конфигурацию Release, либо запустить уже готовый Autoservice.exe из папки release. Перед запуском во всех случаях контейнер autoservice_mssql должен быть запущен, иначе приложение не сможет подключиться к базе.