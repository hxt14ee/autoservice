using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using Autoservice.Data;
using Autoservice.Models;

namespace Autoservice.Forms
{
    public partial class OrdersForm : Form
    {
        private AutoServiceContext context;
        private BindingSource bindingSource;
        private bool isNewOrder = false;

        public OrdersForm()
        {
            InitializeComponent();
            this.Text = "Управление заказами";
            this.Size = new System.Drawing.Size(1100, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            context = new AutoServiceContext();
            bindingSource = new BindingSource();
        }

        private void OrdersForm_Load(object sender, EventArgs e)
        {
            CreateUI();
            LoadData();
        }

        private void CreateUI()
        {
            DataGridView dgvOrders = new DataGridView
            {
                Name = "dgvOrders",
                Dock = DockStyle.Top,
                Height = 250,
                AutoGenerateColumns = true
            };

            Panel pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = System.Drawing.Color.LightGray };

            Button btnAdd = new Button { Text = "Добавить", Width = 100, Left = 10, Top = 10 };
            Button btnEdit = new Button { Text = "Изменить", Width = 100, Left = 120, Top = 10 };
            Button btnDelete = new Button { Text = "Удалить", Width = 100, Left = 230, Top = 10 };
            Button btnRefresh = new Button { Text = "Обновить", Width = 100, Left = 340, Top = 10 };

            btnAdd.Click += (s, e) =>
            {
                isNewOrder = true;
                OpenEditForm(null);
            };

            btnEdit.Click += (s, e) =>
            {
                if (dgvOrders.SelectedRows.Count > 0)
                {
                    isNewOrder = false;
                    int orderId = (int)dgvOrders.SelectedRows[0].Cells[0].Value;
                    OpenEditForm(orderId);
                }
            };

            btnDelete.Click += (s, e) => DeleteOrder(dgvOrders);
            btnRefresh.Click += (s, e) => LoadData();

            pnlButtons.Controls.Add(btnAdd);
            pnlButtons.Controls.Add(btnEdit);
            pnlButtons.Controls.Add(btnDelete);
            pnlButtons.Controls.Add(btnRefresh);

            this.Controls.Add(dgvOrders);
            this.Controls.Add(pnlButtons);

            dgvOrders.DataSource = bindingSource;
        }

        private void LoadData()
        {
            try
            {
                context.Dispose();
                context = new AutoServiceContext();

                var orders = context.Orders
                    .Include(o => o.Car)
                    .Include(o => o.Employee)
                    .ToList()
                    .Select(o => new
                    {
                        o.OrderID,
                        Car = o.Car != null ? o.Car.Brand + " " + o.Car.Model : "—",
                        Employee = o.Employee != null ? o.Employee.FullName : "—",
                        o.Status,
                        o.DateReceived,
                        o.DateCompleted,
                        o.TotalAmount
                    })
                    .ToList();
                bindingSource.DataSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}");
            }
        }

        private void OpenEditForm(int? orderId = null)
        {
            Order order = null;
            bool isNew = orderId == null;

            
            if (!isNew)
            {
                try
                {
                    order = context.Orders.Find(orderId.Value);
                    if (order == null)
                    {
                        MessageBox.Show("Заказ не найден.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке заказа: {ex.Message}");
                    return;
                }
            }

            Form editForm = new Form
            {
                Text = isNew ? "Новый заказ" : "Редактирование заказа",
                Width = 400,
                Height = 380,
                StartPosition = FormStartPosition.CenterParent,
                ShowInTaskbar = false
            };

            Label lblCar = new Label { Text = "Машина:", Left = 10, Top = 10, Width = 100 };
            ComboBox cmbCar = new ComboBox { Left = 120, Top = 10, Width = 250 };

            try
            {
                var cars = context.Cars.ToList();
                cmbCar.DataSource = cars;
                cmbCar.DisplayMember = "Brand";
                cmbCar.ValueMember = "CarID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке машин: {ex.Message}");
                return;
            }

            Label lblEmployee = new Label { Text = "Сотрудник:", Left = 10, Top = 40, Width = 100 };
            ComboBox cmbEmployee = new ComboBox { Left = 120, Top = 40, Width = 250 };

            try
            {
                var employees = context.Employees.ToList();
                cmbEmployee.DataSource = employees;
                cmbEmployee.DisplayMember = "FullName";
                cmbEmployee.ValueMember = "EmployeeID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке сотрудников: {ex.Message}");
                return;
            }

            Label lblDateReceived = new Label { Text = "Дата приема:", Left = 10, Top = 70, Width = 100 };
            DateTimePicker dtpDateReceived = new DateTimePicker { Left = 120, Top = 70, Width = 250 };

            Label lblDateCompleted = new Label { Text = "Дата завершения:", Left = 10, Top = 100, Width = 100 };
            DateTimePicker dtpDateCompleted = new DateTimePicker { Left = 120, Top = 100, Width = 250, Checked = false };

            Label lblStatus = new Label { Text = "Статус:", Left = 10, Top = 130, Width = 100 };
            ComboBox cmbStatus = new ComboBox
            {
                Left = 120,
                Top = 130,
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList  
            };
            
            cmbStatus.Items.AddRange(new[] { "В работе", "Завершён", "Отменён" });

            Label lblTotal = new Label { Text = "Сумма:", Left = 10, Top = 160, Width = 100 };
            TextBox txtTotal = new TextBox { Left = 120, Top = 160, Width = 250 };

            Button btnSave = new Button { Text = "Сохранить", Left = 120, Top = 200, Width = 100 };
            Button btnCancel = new Button { Text = "Отмена", Left = 230, Top = 200, Width = 100 };

            
            if (!isNew && order != null)
            {
                try
                {
                    cmbCar.SelectedValue = order.CarID;
                    cmbEmployee.SelectedValue = order.EmployeeID;
                    dtpDateReceived.Value = order.DateReceived;
                    if (order.DateCompleted.HasValue)
                    {
                        dtpDateCompleted.Value = order.DateCompleted.Value;
                        dtpDateCompleted.Checked = true;
                    }
                    // Ищем точное совпадение статуса
                    foreach (string status in cmbStatus.Items)
                    {
                        if (string.Equals(status, order.Status, StringComparison.OrdinalIgnoreCase))
                        {
                            cmbStatus.SelectedItem = status;
                            break;
                        }
                    }
                    txtTotal.Text = order.TotalAmount.ToString("F2");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при заполнении формы: {ex.Message}");
                }
            }
            else
            {
                dtpDateReceived.Value = DateTime.Now;
                cmbStatus.SelectedIndex = 0; 
                txtTotal.Text = "0.00";
            }

            btnSave.Click += (s, e) =>
            {
                try
                {
                    // Валидация
                    if (cmbCar.SelectedValue == null)
                    {
                        MessageBox.Show("Выберите машину!");
                        return;
                    }
                    if (cmbEmployee.SelectedValue == null)
                    {
                        MessageBox.Show("Выберите сотрудника!");
                        return;
                    }
                    if (cmbStatus.SelectedItem == null)
                    {
                        MessageBox.Show("Выберите статус!");
                        return;
                    }

                    // ВАЛИДАЦИЯ STATUS 
                    var allowedStatuses = new[] { "В работе", "Завершён", "Отменён" };
                    string statusValue = cmbStatus.SelectedItem.ToString().Trim();
                    if (!allowedStatuses.Contains(statusValue))
                    {
                        MessageBox.Show($"Неверный статус: '{statusValue}'. Допустимые: 'В работе', 'Завершён', 'Отменён'");
                        return;
                    }

                    if (!decimal.TryParse(txtTotal.Text, out decimal totalAmount) || totalAmount < 0)
                    {
                        MessageBox.Show("Введи корректную сумму (число >= 0)!");
                        return;
                    }

                    
                    if (isNew)
                    {
                        order = new Order();
                        context.Orders.Add(order);
                    }

                    // Устанавливаем значения
                    order.CarID = (int)cmbCar.SelectedValue;
                    order.EmployeeID = (int)cmbEmployee.SelectedValue;
                    order.DateReceived = dtpDateReceived.Value;
                    order.DateCompleted = dtpDateCompleted.Checked ? (DateTime?)dtpDateCompleted.Value : null;
                    order.Status = statusValue;  
                    order.TotalAmount = totalAmount;

                    if (statusValue == "Завершён")
                    {
                        order.DateCompleted = DateTime.Now;
                    }
                    else if (statusValue == "Отменён")
                    {
                        order.DateCompleted = DateTime.Now;
                    }
                    else
                    {
                        order.DateCompleted = null;
                    }
                    context.SaveChanges();
                    MessageBox.Show("Заказ сохранен успешно!");
                    LoadData();
                    editForm.Close();
                }
                catch (Exception ex)
                {
                    string errorMsg = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        errorMsg += "\n\nВнутренняя ошибка: " + inner.Message;
                        inner = inner.InnerException;
                    }
                    MessageBox.Show("Ошибка при сохранении:\n" + errorMsg);
                }
            };

            btnCancel.Click += (s, e) => editForm.Close();

            editForm.Controls.Add(lblCar);
            editForm.Controls.Add(cmbCar);
            editForm.Controls.Add(lblEmployee);
            editForm.Controls.Add(cmbEmployee);
            editForm.Controls.Add(lblDateReceived);
            editForm.Controls.Add(dtpDateReceived);
            editForm.Controls.Add(lblDateCompleted);
            editForm.Controls.Add(dtpDateCompleted);
            editForm.Controls.Add(lblStatus);
            editForm.Controls.Add(cmbStatus);
            editForm.Controls.Add(lblTotal);
            editForm.Controls.Add(txtTotal);
            editForm.Controls.Add(btnSave);
            editForm.Controls.Add(btnCancel);

            editForm.ShowDialog();
        }

        private void DeleteOrder(DataGridView dgv)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int orderId = (int)dgv.SelectedRows[0].Cells[0].Value;
                    var order = context.Orders.Find(orderId);
                    if (order != null)
                    {
                        context.Orders.Remove(order);
                        context.SaveChanges();
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void OrdersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            context?.Dispose();
        }
    }
}
