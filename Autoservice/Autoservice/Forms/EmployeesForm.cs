using System;
using System.Linq;
using System.Windows.Forms;
using Autoservice.Data;
using Autoservice.Models;

namespace Autoservice.Forms
{
    public partial class EmployeesForm : Form
    {
        private AutoServiceContext context;
        private BindingSource bindingSource;

        public EmployeesForm()
        {
            InitializeComponent();
            this.Text = "Управление сотрудниками";
            this.Size = new System.Drawing.Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            context = new AutoServiceContext();
            bindingSource = new BindingSource();
        }

        private void EmployeesForm_Load(object sender, EventArgs e)
        {
            CreateUI();
            LoadData();
        }

        private void CreateUI()
        {
            DataGridView dgvEmployees = new DataGridView
            {
                Name = "dgvEmployees",
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
                if (dgvEmployees.SelectedRows.Count > 0)
                {
                    int empId = (int)dgvEmployees.SelectedRows[0].Cells[0].Value;
                    var emp = context.Employees.Find(empId);
                    OpenEditForm(emp);
                }
            };
            btnDelete.Click += (s, e) => DeleteEmployee(dgvEmployees);
            btnRefresh.Click += (s, e) => LoadData();

            pnlButtons.Controls.Add(btnAdd);
            pnlButtons.Controls.Add(btnEdit);
            pnlButtons.Controls.Add(btnDelete);
            pnlButtons.Controls.Add(btnRefresh);

            this.Controls.Add(dgvEmployees);
            this.Controls.Add(pnlButtons);

            dgvEmployees.DataSource = bindingSource;
        }

        private void LoadData()
        {
            try
            {
                context.Dispose();
                context = new AutoServiceContext();

                var employees = context.Employees.Select(e => new
                {
                    e.EmployeeID,
                    e.FullName,
                    e.Position,
                    e.Phone,
                    e.Salary
                }).ToList();

                bindingSource.DataSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }



        private void OpenEditForm(Employee emp)
        {
            Form editForm = new Form
            {
                Text = emp == null ? "Новый сотрудник" : "Редактирование",
                Width = 400,
                Height = 280,
                StartPosition = FormStartPosition.CenterParent
            };

            Label lblName = new Label { Text = "ФИО:", Left = 10, Top = 10, Width = 100 };
            TextBox txtName = new TextBox { Left = 120, Top = 10, Width = 250 };

            Label lblPosition = new Label { Text = "Должность:", Left = 10, Top = 40, Width = 100 };
            TextBox txtPosition = new TextBox { Left = 120, Top = 40, Width = 250 };

            Label lblPhone = new Label { Text = "Телефон:", Left = 10, Top = 70, Width = 100 };
            TextBox txtPhone = new TextBox { Left = 120, Top = 70, Width = 250 };

            Label lblSalary = new Label { Text = "Зарплата:", Left = 10, Top = 100, Width = 100 };
            TextBox txtSalary = new TextBox { Left = 120, Top = 100, Width = 250 };

            Button btnSave = new Button { Text = "Сохранить", Left = 120, Top = 150, Width = 100 };
            Button btnCancel = new Button { Text = "Отмена", Left = 230, Top = 150, Width = 100 };

            if (emp != null)
            {
                txtName.Text = emp.FullName;
                txtPosition.Text = emp.Position;
                txtPhone.Text = emp.Phone;
                txtSalary.Text = emp.Salary.ToString();
            }

            btnSave.Click += (s, e) =>
            {
                try
                {
                    if (emp == null)
                    {
                        emp = new Employee();
                        context.Employees.Add(emp);
                    }

                    emp.FullName = txtName.Text;
                    emp.Position = txtPosition.Text;
                    emp.Phone = txtPhone.Text;
                    emp.Salary = decimal.Parse(txtSalary.Text);

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
            editForm.Controls.Add(lblPosition);
            editForm.Controls.Add(txtPosition);
            editForm.Controls.Add(lblPhone);
            editForm.Controls.Add(txtPhone);
            editForm.Controls.Add(lblSalary);
            editForm.Controls.Add(txtSalary);
            editForm.Controls.Add(btnSave);
            editForm.Controls.Add(btnCancel);

            editForm.ShowDialog();
        }

        private void DeleteEmployee(DataGridView dgv)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите сотрудника");
                return;
            }

            if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int empId = (int)dgv.SelectedRows[0].Cells[0].Value;
                    var emp = context.Employees.Find(empId);
                    context.Employees.Remove(emp);
                    context.SaveChanges();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void EmployeesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            context.Dispose();
        }
    }
}
