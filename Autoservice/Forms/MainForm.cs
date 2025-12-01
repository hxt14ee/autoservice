using System;
using System.Windows.Forms;
using Autoservice.Forms;

namespace Autoservice.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Text = "Автосервис - Главное меню";
            this.Size = new System.Drawing.Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CreateMenu();
        }

        private void CreateMenu()
        {
            // Создаём кнопки для навигации
            Button btnClients = new Button { Text = "Клиенты", Dock = DockStyle.Top, Height = 40 };
            Button btnCars = new Button { Text = "Автомобили", Dock = DockStyle.Top, Height = 40 };
            Button btnEmployees = new Button { Text = "Сотрудники", Dock = DockStyle.Top, Height = 40 };
            Button btnParts = new Button { Text = "Запчасти", Dock = DockStyle.Top, Height = 40 };
            Button btnServices = new Button { Text = "Услуги", Dock = DockStyle.Top, Height = 40 };
            Button btnOrders = new Button { Text = "Заказы", Dock = DockStyle.Top, Height = 40 };
            Button btnQueryBuilder = new Button { Text = "Конструктор запросов", Dock = DockStyle.Top, Height = 40 };
            Button btnBackup = new Button { Text = "Резервная копия", Dock = DockStyle.Top, Height = 40 };

            btnClients.Click += (s, e) => OpenForm(new ClientsForm());
            btnCars.Click += (s, e) => OpenForm(new CarsForm());
            btnEmployees.Click += (s, e) => OpenForm(new EmployeesForm());
            btnParts.Click += (s, e) => OpenForm(new PartsForm());
            btnServices.Click += (s, e) => OpenForm(new ServicesForm());
            btnOrders.Click += (s, e) => OpenForm(new OrdersForm());
            btnQueryBuilder.Click += (s, e) => OpenForm(new QueryBuilderForm());
            btnBackup.Click += (s, e) => OpenForm(new BackupRestoreForm());

            this.Controls.Add(btnClients);
            this.Controls.Add(btnCars);
            this.Controls.Add(btnEmployees);
            this.Controls.Add(btnParts);
            this.Controls.Add(btnServices);
            this.Controls.Add(btnOrders);
            this.Controls.Add(btnQueryBuilder);
            this.Controls.Add(btnBackup);
        }

        private void OpenForm(Form form)
        {
            form.ShowDialog();
        }
    }
}
