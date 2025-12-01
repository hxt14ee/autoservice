using System;
using System.Linq;
using System.Windows.Forms;
using Autoservice.Data;
using Autoservice.Models;

namespace Autoservice.Forms
{
    public partial class ServicesForm : Form
    {
        private AutoServiceContext context;
        private BindingSource bindingSource;

        public ServicesForm()
        {
            InitializeComponent();
            this.Text = "Управление услугами";
            this.Size = new System.Drawing.Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            context = new AutoServiceContext();
            bindingSource = new BindingSource();
        }

        private void ServicesForm_Load(object sender, EventArgs e)
        {
            CreateUI();
            LoadData();
        }

        private void CreateUI()
        {
            DataGridView dgvServices = new DataGridView
            {
                Name = "dgvServices",
                Dock = DockStyle.Top,
                Height = 250,
                AutoGenerateColumns = true
            };

            Panel pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = System.Drawing.Color.LightGray };

            Button btnAdd = new Button { Text = "Добавить", Width = 100, Left = 10, Top = 10 };
            Button btnEdit = new Button { Text = "Изменить", Width = 100, Left = 120, Top = 10 };
            Button btnDelete = new Button { Text = "Удалить", Width = 100, Left = 230, Top = 10 };
            Button btnRefresh = new Button { Text = "Обновить", Width = 100, Left = 340, Top = 10 };

            btnAdd.Click += (s, e) => OpenEditForm(null);
            btnEdit.Click += (s, e) =>
            {
                if (dgvServices.SelectedRows.Count > 0)
                {
                    int svcId = (int)dgvServices.SelectedRows[0].Cells[0].Value;
                    var svc = context.Services.Find(svcId);
                    OpenEditForm(svc);
                }
            };
            btnDelete.Click += (s, e) => DeleteService(dgvServices);
            btnRefresh.Click += (s, e) => LoadData();

            pnlButtons.Controls.Add(btnAdd);
            pnlButtons.Controls.Add(btnEdit);
            pnlButtons.Controls.Add(btnDelete);
            pnlButtons.Controls.Add(btnRefresh);

            this.Controls.Add(dgvServices);
            this.Controls.Add(pnlButtons);

            dgvServices.DataSource = bindingSource;
        }

        private void LoadData()
        {
            try
            {
                context.Dispose();
                context = new AutoServiceContext();

                var services = context.Services.Select(s => new
                {
                    s.ServiceID,
                    s.ServiceName,
                    s.Description,
                    s.Price
                }).ToList();

                bindingSource.DataSource = services;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }



        private void OpenEditForm(Service service)
        {
            Form editForm = new Form
            {
                Text = service == null ? "Новая услуга" : "Редактирование",
                Width = 400,
                Height = 280,
                StartPosition = FormStartPosition.CenterParent
            };

            Label lblName = new Label { Text = "Название:", Left = 10, Top = 10, Width = 100 };
            TextBox txtName = new TextBox { Left = 120, Top = 10, Width = 250 };

            Label lblDescription = new Label { Text = "Описание:", Left = 10, Top = 40, Width = 100 };
            TextBox txtDescription = new TextBox { Left = 120, Top = 40, Width = 250 };

            Label lblPrice = new Label { Text = "Цена:", Left = 10, Top = 70, Width = 100 };
            TextBox txtPrice = new TextBox { Left = 120, Top = 70, Width = 250 };

            Button btnSave = new Button { Text = "Сохранить", Left = 120, Top = 120, Width = 100 };
            Button btnCancel = new Button { Text = "Отмена", Left = 230, Top = 120, Width = 100 };

            if (service != null)
            {
                txtName.Text = service.ServiceName;
                txtDescription.Text = service.Description;
                txtPrice.Text = service.Price.ToString();
            }

            btnSave.Click += (s, e) =>
            {
                try
                {
                    if (service == null)
                    {
                        service = new Service();
                        context.Services.Add(service);
                    }

                    service.ServiceName = txtName.Text;
                    service.Description = txtDescription.Text;
                    service.Price = decimal.Parse(txtPrice.Text);

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

            editForm.Controls.Add(lblName);
            editForm.Controls.Add(txtName);
            editForm.Controls.Add(lblDescription);
            editForm.Controls.Add(txtDescription);
            editForm.Controls.Add(lblPrice);
            editForm.Controls.Add(txtPrice);
            editForm.Controls.Add(btnSave);
            editForm.Controls.Add(btnCancel);

            editForm.ShowDialog();
        }

        private void DeleteService(DataGridView dgv)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите услугу");
                return;
            }

            if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int svcId = (int)dgv.SelectedRows[0].Cells[0].Value;
                    var service = context.Services.Find(svcId);
                    context.Services.Remove(service);
                    context.SaveChanges();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void ServicesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            context.Dispose();
        }
    }
}
