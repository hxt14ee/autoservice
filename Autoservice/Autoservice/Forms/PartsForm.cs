using System;
using System.Linq;
using System.Windows.Forms;
using Autoservice.Data;
using Autoservice.Models;

namespace Autoservice.Forms
{
    public partial class PartsForm : Form
    {
        private AutoServiceContext context;
        private BindingSource bindingSource;

        public PartsForm()
        {
            InitializeComponent();
            this.Text = "Управление запчастями";
            this.Size = new System.Drawing.Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            context = new AutoServiceContext();
            bindingSource = new BindingSource();
        }

        private void PartsForm_Load(object sender, EventArgs e)
        {
            CreateUI();
            LoadData();
        }

        private void CreateUI()
        {
            DataGridView dgvParts = new DataGridView
            {
                Name = "dgvParts",
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
                if (dgvParts.SelectedRows.Count > 0)
                {
                    int partId = (int)dgvParts.SelectedRows[0].Cells[0].Value;
                    var part = context.Parts.Find(partId);
                    OpenEditForm(part);
                }
            };
            btnDelete.Click += (s, e) => DeletePart(dgvParts);
            btnRefresh.Click += (s, e) => LoadData();

            pnlButtons.Controls.Add(btnAdd);
            pnlButtons.Controls.Add(btnEdit);
            pnlButtons.Controls.Add(btnDelete);
            pnlButtons.Controls.Add(btnRefresh);

            this.Controls.Add(dgvParts);
            this.Controls.Add(pnlButtons);

            dgvParts.DataSource = bindingSource;
        }

        private void LoadData()
        {
            try
            {
                context.Dispose();
                context = new AutoServiceContext();

                var parts = context.Parts.Select(p => new
                {
                    p.PartID,
                    p.PartName,
                    p.PartNumber,
                    p.Manufacturer,
                    p.Price
                }).ToList();

                bindingSource.DataSource = parts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }



        private void OpenEditForm(Part part)
        {
            Form editForm = new Form
            {
                Text = part == null ? "Новая запчасть" : "Редактирование",
                Width = 400,
                Height = 280,
                StartPosition = FormStartPosition.CenterParent
            };

            Label lblName = new Label { Text = "Название:", Left = 10, Top = 10, Width = 100 };
            TextBox txtName = new TextBox { Left = 120, Top = 10, Width = 250 };

            Label lblNumber = new Label { Text = "Номер:", Left = 10, Top = 40, Width = 100 };
            TextBox txtNumber = new TextBox { Left = 120, Top = 40, Width = 250 };

            Label lblManufacturer = new Label { Text = "Производитель:", Left = 10, Top = 70, Width = 100 };
            TextBox txtManufacturer = new TextBox { Left = 120, Top = 70, Width = 250 };

            Label lblPrice = new Label { Text = "Цена:", Left = 10, Top = 100, Width = 100 };
            TextBox txtPrice = new TextBox { Left = 120, Top = 100, Width = 250 };

            Button btnSave = new Button { Text = "Сохранить", Left = 120, Top = 150, Width = 100 };
            Button btnCancel = new Button { Text = "Отмена", Left = 230, Top = 150, Width = 100 };

            if (part != null)
            {
                txtName.Text = part.PartName;
                txtNumber.Text = part.PartNumber;
                txtManufacturer.Text = part.Manufacturer;
                txtPrice.Text = part.Price.ToString();
            }

            btnSave.Click += (s, e) =>
            {
                try
                {
                    if (part == null)
                    {
                        part = new Part();
                        context.Parts.Add(part);
                    }

                    part.PartName = txtName.Text;
                    part.PartNumber = txtNumber.Text;
                    part.Manufacturer = txtManufacturer.Text;
                    part.Price = decimal.Parse(txtPrice.Text);

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
            editForm.Controls.Add(lblNumber);
            editForm.Controls.Add(txtNumber);
            editForm.Controls.Add(lblManufacturer);
            editForm.Controls.Add(txtManufacturer);
            editForm.Controls.Add(lblPrice);
            editForm.Controls.Add(txtPrice);
            editForm.Controls.Add(btnSave);
            editForm.Controls.Add(btnCancel);

            editForm.ShowDialog();
        }

        private void DeletePart(DataGridView dgv)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите запчасть");
                return;
            }

            if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    int partId = (int)dgv.SelectedRows[0].Cells[0].Value;
                    var part = context.Parts.Find(partId);
                    context.Parts.Remove(part);
                    context.SaveChanges();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void PartsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            context.Dispose();
        }
    }
}
