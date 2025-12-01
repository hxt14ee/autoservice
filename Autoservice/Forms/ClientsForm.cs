using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using Autoservice.Data;
using Autoservice.Models;

namespace Autoservice.Forms
{
    public partial class ClientsForm : Form
    {
        private AutoServiceContext context;
        private BindingSource bindingSource;

        public ClientsForm()
        {
            InitializeComponent();
            this.Text = "Управление клиентами";
            this.Size = new System.Drawing.Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            context = new AutoServiceContext();
            bindingSource = new BindingSource();
        }

        private void ClientsForm_Load(object sender, EventArgs e)
        {
            CreateUI();
            LoadData();
        }

        private void CreateUI()
        {
            
            DataGridView dgvClients = new DataGridView
            {
                Name = "dgvClients",
                Dock = DockStyle.Top,
                Height = 250,
                AutoGenerateColumns = true,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // панель с кнопками
            Panel pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = System.Drawing.Color.LightGray };

            Button btnAdd = new Button { Text = "Добавить", Width = 100, Left = 10, Top = 10 };
            Button btnEdit = new Button { Text = "Изменить", Width = 100, Left = 120, Top = 10 };
            Button btnDelete = new Button { Text = "Удалить", Width = 100, Left = 230, Top = 10 };
            Button btnRefresh = new Button { Text = "Обновить", Width = 100, Left = 340, Top = 10 };

            btnAdd.Click += (s, e) => OpenEditForm(null);
            btnEdit.Click += (s, e) =>
            {
                if (dgvClients.SelectedRows.Count > 0)
                {
                    int clientId = (int)dgvClients.SelectedRows[0].Cells[0].Value;
                    var client = context.Clients.Find(clientId);
                    OpenEditForm(client);
                }
            };
            btnDelete.Click += (s, e) => DeleteClient(dgvClients);
            btnRefresh.Click += (s, e) => LoadData();

            pnlButtons.Controls.Add(btnAdd);
            pnlButtons.Controls.Add(btnEdit);
            pnlButtons.Controls.Add(btnDelete);
            pnlButtons.Controls.Add(btnRefresh);

            this.Controls.Add(dgvClients);
            this.Controls.Add(pnlButtons);

            dgvClients.DataSource = bindingSource;
        }

        private void LoadData()
        {
            try
            {
                context.Dispose();
                context = new AutoServiceContext();

                var clients = context.Clients.Select(c => new
                {
                    c.ClientID,
                    c.FullName,
                    c.Phone,
                    c.Email,
                    c.Address,
                    c.ClientType
                }).ToList();

                bindingSource.DataSource = clients;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }



        private void OpenEditForm(Client client)
        {
            Form editForm = new Form
            {
                Text = client == null ? "Новый клиент" : "Редактирование клиента",
                Width = 400,
                Height = 300,
                StartPosition = FormStartPosition.CenterParent,
                ShowInTaskbar = false
            };

            // поля
            Label lblFullName = new Label { Text = "ФИО:", Left = 10, Top = 10, Width = 100 };
            TextBox txtFullName = new TextBox { Left = 120, Top = 10, Width = 250 };

            Label lblPhone = new Label { Text = "Телефон:", Left = 10, Top = 40, Width = 100 };
            TextBox txtPhone = new TextBox { Left = 120, Top = 40, Width = 250 };

            Label lblEmail = new Label { Text = "Email:", Left = 10, Top = 70, Width = 100 };
            TextBox txtEmail = new TextBox { Left = 120, Top = 70, Width = 250 };

            Label lblAddress = new Label { Text = "Адрес:", Left = 10, Top = 100, Width = 100 };
            TextBox txtAddress = new TextBox { Left = 120, Top = 100, Width = 250 };

            Label lblClientType = new Label { Text = "Тип:", Left = 10, Top = 130, Width = 100 };
            ComboBox cmbClientType = new ComboBox
            {
                Left = 120,
                Top = 130,
                Width = 250,
                Items = { "Физическое лицо", "Юридическое лицо" }
            };

            // кнопки
            Button btnSave = new Button { Text = "Сохранить", Left = 120, Top = 170, Width = 100 };
            Button btnCancel = new Button { Text = "Отмена", Left = 230, Top = 170, Width = 100 };

            if (client != null)
            {
                txtFullName.Text = client.FullName;
                txtPhone.Text = client.Phone;
                txtEmail.Text = client.Email;
                txtAddress.Text = client.Address;
                cmbClientType.SelectedItem = client.ClientType;
            }

            btnSave.Click += (s, e) =>
            {
                try
                {
                    if (client == null)
                    {
                        client = new Client();
                        context.Clients.Add(client);
                    }

                    client.FullName = txtFullName.Text;
                    client.Phone = txtPhone.Text;
                    client.Email = txtEmail.Text;
                    client.Address = txtAddress.Text;
                    client.ClientType = cmbClientType.SelectedItem?.ToString();

                    context.SaveChanges();
                    MessageBox.Show("Клиент сохранен успешно!");
                    LoadData();
                    editForm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            };

            btnCancel.Click += (s, e) => editForm.Close();

            editForm.Controls.Add(lblFullName);
            editForm.Controls.Add(txtFullName);
            editForm.Controls.Add(lblPhone);
            editForm.Controls.Add(txtPhone);
            editForm.Controls.Add(lblEmail);
            editForm.Controls.Add(txtEmail);
            editForm.Controls.Add(lblAddress);
            editForm.Controls.Add(txtAddress);
            editForm.Controls.Add(lblClientType);
            editForm.Controls.Add(cmbClientType);
            editForm.Controls.Add(btnSave);
            editForm.Controls.Add(btnCancel);

            editForm.ShowDialog();
        }

        private void DeleteClient(DataGridView dgv)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите клиента для удаления");
                return;
            }

            if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int clientId = (int)dgv.SelectedRows[0].Cells[0].Value;
                    var client = context.Clients.Find(clientId);
                    context.Clients.Remove(client);
                    context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Клиент удален!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void ClientsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            context.Dispose();
        }
    }
}
