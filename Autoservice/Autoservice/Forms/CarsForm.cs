using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using Autoservice.Data;
using Autoservice.Models;

namespace Autoservice.Forms
{
    public partial class CarsForm : Form
    {
        private AutoServiceContext context;
        private BindingSource bindingSource;

        public CarsForm()
        {
            InitializeComponent();
            this.Text = "Управление автомобилями";
            this.Size = new System.Drawing.Size(1000, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            context = new AutoServiceContext();
            bindingSource = new BindingSource();
        }

        private void CarsForm_Load(object sender, EventArgs e)
        {
            CreateUI();
            LoadData();
        }

        private void CreateUI()
        {
            DataGridView dgvCars = new DataGridView
            {
                Name = "dgvCars",
                Dock = DockStyle.Top,
                Height = 250,
                AutoGenerateColumns = true,
                ReadOnly = false
            };

            Panel pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = System.Drawing.Color.LightGray };

            Button btnAdd = new Button { Text = "Добавить", Width = 100, Left = 10, Top = 10 };
            Button btnEdit = new Button { Text = "Изменить", Width = 100, Left = 120, Top = 10 };
            Button btnDelete = new Button { Text = "Удалить", Width = 100, Left = 230, Top = 10 };
            Button btnRefresh = new Button { Text = "Обновить", Width = 100, Left = 340, Top = 10 };

            btnAdd.Click += (s, e) => OpenEditForm(null);
            btnEdit.Click += (s, e) =>
            {
                if (dgvCars.SelectedRows.Count > 0)
                {
                    int carId = (int)dgvCars.SelectedRows[0].Cells[0].Value;
                    var car = context.Cars.Find(carId);
                    OpenEditForm(car);
                }
            };
            btnDelete.Click += (s, e) => DeleteCar(dgvCars);
            btnRefresh.Click += (s, e) => LoadData();

            pnlButtons.Controls.Add(btnAdd);
            pnlButtons.Controls.Add(btnEdit);
            pnlButtons.Controls.Add(btnDelete);
            pnlButtons.Controls.Add(btnRefresh);

            this.Controls.Add(dgvCars);
            this.Controls.Add(pnlButtons);

            dgvCars.DataSource = bindingSource;
        }

        private void LoadData()
        {
            try
            {
                context.Dispose();
                context = new AutoServiceContext();

                var cars = context.Cars
                    .Include(c => c.Client) 
                    .Select(c => new
                    {
                        c.CarID,
                        c.Brand,
                        c.Model,
                        c.Year,
                        c.VIN,
                        ClientName = c.Client.FullName 
                    }).ToList();

                bindingSource.DataSource = cars;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }



        private void OpenEditForm(Car car)
        {
            Form editForm = new Form
            {
                Text = car == null ? "Новый автомобиль" : "Редактирование",
                Width = 400,
                Height = 320,
                StartPosition = FormStartPosition.CenterParent
            };

            Label lblBrand = new Label { Text = "Марка:", Left = 10, Top = 10, Width = 100 };
            TextBox txtBrand = new TextBox { Left = 120, Top = 10, Width = 250 };

            Label lblModel = new Label { Text = "Модель:", Left = 10, Top = 40, Width = 100 };
            TextBox txtModel = new TextBox { Left = 120, Top = 40, Width = 250 };

            Label lblYear = new Label { Text = "Год:", Left = 10, Top = 70, Width = 100 };
            TextBox txtYear = new TextBox { Left = 120, Top = 70, Width = 250 };

            Label lblLicense = new Label { Text = "Гос. номер:", Left = 10, Top = 100, Width = 100 };
            TextBox txtLicense = new TextBox { Left = 120, Top = 100, Width = 250 };

            Label lblVIN = new Label { Text = "VIN:", Left = 10, Top = 130, Width = 100 };
            TextBox txtVIN = new TextBox { Left = 120, Top = 130, Width = 250 };

            Label lblClient = new Label { Text = "Клиент:", Left = 10, Top = 160, Width = 100 };
            ComboBox cmbClient = new ComboBox { Left = 120, Top = 160, Width = 250 };

            var clients = context.Clients.ToList();
            cmbClient.DataSource = clients;
            cmbClient.DisplayMember = "FullName";
            cmbClient.ValueMember = "ClientID";

            Button btnSave = new Button { Text = "Сохранить", Left = 120, Top = 200, Width = 100 };
            Button btnCancel = new Button { Text = "Отмена", Left = 230, Top = 200, Width = 100 };

            if (car != null)
            {
                txtBrand.Text = car.Brand;
                txtModel.Text = car.Model;
                txtYear.Text = car.Year.ToString();
                txtLicense.Text = car.LicensePlate;
                txtVIN.Text = car.VIN;
                cmbClient.SelectedValue = car.ClientID;
            }

            btnSave.Click += (s, e) =>
            {
                try
                {
                    if (car == null)
                    {
                        car = new Car();
                        context.Cars.Add(car);
                    }

                    car.Brand = txtBrand.Text;
                    car.Model = txtModel.Text;
                    car.Year = int.Parse(txtYear.Text);
                    car.LicensePlate = txtLicense.Text;
                    car.VIN = txtVIN.Text;
                    car.ClientID = (int)cmbClient.SelectedValue;

                    context.SaveChanges();
                    MessageBox.Show("Сохранено!");
                    LoadData();
                    editForm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            };

            btnCancel.Click += (s, e) => editForm.Close();

            editForm.Controls.Add(lblBrand);
            editForm.Controls.Add(txtBrand);
            editForm.Controls.Add(lblModel);
            editForm.Controls.Add(txtModel);
            editForm.Controls.Add(lblYear);
            editForm.Controls.Add(txtYear);
            editForm.Controls.Add(lblLicense);
            editForm.Controls.Add(txtLicense);
            editForm.Controls.Add(lblVIN);
            editForm.Controls.Add(txtVIN);
            editForm.Controls.Add(lblClient);
            editForm.Controls.Add(cmbClient);
            editForm.Controls.Add(btnSave);
            editForm.Controls.Add(btnCancel);

            editForm.ShowDialog();
        }

        private void DeleteCar(DataGridView dgv)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите машину");
                return;
            }

            if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int carId = (int)dgv.SelectedRows[0].Cells[0].Value;
                    var car = context.Cars.Find(carId);
                    context.Cars.Remove(car);
                    context.SaveChanges();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void CarsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            context.Dispose();
        }
    }
}
