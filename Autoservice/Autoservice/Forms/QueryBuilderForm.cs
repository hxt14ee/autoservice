using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autoservice.Data;
using Newtonsoft.Json;

namespace Autoservice.Forms
{
    public class QueryBuilderForm : Form
    {
        private AutoServiceContext context;
        private DataGridView dgvResults;
        private ComboBox cmbTable;
        private Panel pnlFilters;
        private List<object> currentData;

        public QueryBuilderForm()
        {
            this.Text = "Конструктор запросов";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += (s, e) => context?.Dispose();

            context = new AutoServiceContext();

            // Выбор таблицы
            cmbTable = new ComboBox
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(200, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Заказы", "Клиенты", "Автомобили", "Сотрудники", "Запчасти", "Услуги" }
            };
            cmbTable.SelectedIndex = 0;
            cmbTable.SelectedIndexChanged += (s, e) => LoadFilters();

            // Панель фильтров
            pnlFilters = new Panel
            {
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(870, 80),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Таблица результатов
            dgvResults = new DataGridView
            {
                Location = new System.Drawing.Point(10, 130),
                Size = new System.Drawing.Size(870, 310),
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Кнопки
            var btnExecute = new Button { Text = "Выполнить", Location = new System.Drawing.Point(10, 450), Size = new System.Drawing.Size(100, 30) };
            var btnClear = new Button { Text = "Очистить", Location = new System.Drawing.Point(120, 450), Size = new System.Drawing.Size(100, 30) };
            var btnExportCsv = new Button { Text = "Экспорт CSV", Location = new System.Drawing.Point(230, 450), Size = new System.Drawing.Size(100, 30) };
            var btnExportJson = new Button { Text = "Экспорт JSON", Location = new System.Drawing.Point(340, 450), Size = new System.Drawing.Size(100, 30) };
            var btnClose = new Button { Text = "Закрыть", Location = new System.Drawing.Point(780, 450), Size = new System.Drawing.Size(100, 30) };

            btnExecute.Click += (s, e) =>
            {
                try
                {
                    currentData = GetTableData(cmbTable.SelectedItem.ToString());
                    dgvResults.DataSource = currentData;
                    MessageBox.Show($"Записей: {currentData.Count}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            };

            btnClear.Click += (s, e) =>
            {
                foreach (var ctrl in pnlFilters.Controls)
                {
                    if (ctrl is TextBox) ((TextBox)ctrl).Text = "";
                    if (ctrl is ComboBox) ((ComboBox)ctrl).SelectedIndex = 0;
                    if (ctrl is NumericUpDown) ((NumericUpDown)ctrl).Value = 0;
                }
                dgvResults.DataSource = null;
                currentData = null;
            };

            btnExportCsv.Click += (s, e) => ExportCsv();
            btnExportJson.Click += (s, e) => ExportJson();
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { cmbTable, pnlFilters, dgvResults, btnExecute, btnClear, btnExportCsv, btnExportJson, btnClose });

            LoadFilters();
        }

        private void LoadFilters()
        {
            pnlFilters.Controls.Clear();
            string table = cmbTable.SelectedItem.ToString();

            switch (table)
            {
                case "Заказы":
                    var lblStatus = new Label { Text = "Статус:", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(50, 20) };
                    var cmbStatus = new ComboBox { Location = new System.Drawing.Point(65, 7), Size = new System.Drawing.Size(150, 23), Items = { "Все", "В работе", "Завершён", "Отменён" }, DropDownStyle = ComboBoxStyle.DropDownList };
                    cmbStatus.SelectedIndex = 0;

                    var lblSum = new Label { Text = "Сумма от:", Location = new System.Drawing.Point(225, 10), Size = new System.Drawing.Size(60, 20) };
                    var numSum = new NumericUpDown { Location = new System.Drawing.Point(285, 7), Size = new System.Drawing.Size(80, 23), Minimum = 0, Maximum = 1000000 };

                    pnlFilters.Controls.AddRange(new Control[] { lblStatus, cmbStatus, lblSum, numSum });
                    break;

                case "Клиенты":
                    var lblType = new Label { Text = "Тип:", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(40, 20) };
                    var cmbType = new ComboBox { Location = new System.Drawing.Point(50, 7), Size = new System.Drawing.Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
                    cmbType.Items.Add("Все");
                    cmbType.Items.AddRange(context.Clients.Select(c => c.ClientType).Distinct().ToArray());
                    cmbType.SelectedIndex = 0;

                    pnlFilters.Controls.AddRange(new Control[] { lblType, cmbType });
                    break;

                case "Автомобили":
                    var lblBrand = new Label { Text = "Марка:", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(50, 20) };
                    var cmbBrand = new ComboBox { Location = new System.Drawing.Point(65, 7), Size = new System.Drawing.Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
                    cmbBrand.Items.Add("Все");
                    cmbBrand.Items.AddRange(context.Cars.Select(c => c.Brand).Distinct().ToArray());
                    cmbBrand.SelectedIndex = 0;

                    pnlFilters.Controls.AddRange(new Control[] { lblBrand, cmbBrand });
                    break;

                case "Сотрудники":
                    var lblPos = new Label { Text = "Должность:", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(70, 20) };
                    var cmbPos = new ComboBox { Location = new System.Drawing.Point(80, 7), Size = new System.Drawing.Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
                    cmbPos.Items.Add("Все");
                    cmbPos.Items.AddRange(context.Employees.Select(e => e.Position).Distinct().ToArray());
                    cmbPos.SelectedIndex = 0;

                    var lblSalary = new Label { Text = "Зарплата от:", Location = new System.Drawing.Point(240, 10), Size = new System.Drawing.Size(70, 20) };
                    var numSalary = new NumericUpDown { Location = new System.Drawing.Point(310, 7), Size = new System.Drawing.Size(100, 23), Minimum = 0, Maximum = 100000 };

                    pnlFilters.Controls.AddRange(new Control[] { lblPos, cmbPos, lblSalary, numSalary });
                    break;

                case "Запчасти":
                    var lblMfr = new Label { Text = "Производитель:", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(80, 20) };
                    var cmbMfr = new ComboBox { Location = new System.Drawing.Point(90, 7), Size = new System.Drawing.Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
                    cmbMfr.Items.Add("Все");
                    cmbMfr.Items.AddRange(context.Parts.Select(p => p.Manufacturer).Distinct().ToArray());
                    cmbMfr.SelectedIndex = 0;

                    pnlFilters.Controls.AddRange(new Control[] { lblMfr, cmbMfr });
                    break;

                case "Услуги":
                    var lblSvc = new Label { Text = "Услуга:", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(50, 20) };
                    var cmbSvc = new ComboBox { Location = new System.Drawing.Point(60, 7), Size = new System.Drawing.Size(250, 23), DropDownStyle = ComboBoxStyle.DropDownList };
                    cmbSvc.Items.Add("Все");
                    cmbSvc.Items.AddRange(context.Services.Select(s => s.ServiceName).Distinct().ToArray());
                    cmbSvc.SelectedIndex = 0;

                    pnlFilters.Controls.AddRange(new Control[] { lblSvc, cmbSvc });
                    break;
            }
        }

        private List<object> GetTableData(string table)
        {
            switch (table)
            {
                case "Заказы":
                    var cmbStatus = pnlFilters.Controls.OfType<ComboBox>().FirstOrDefault();
                    var numSum = pnlFilters.Controls.OfType<NumericUpDown>().FirstOrDefault();

                    var orders = context.Orders
                        .Include(o => o.Car)
                        .Include(o => o.Employee)
                        .ToList();

                    if (cmbStatus?.SelectedIndex > 0)
                        orders = orders.Where(o => o.Status == cmbStatus.Items[cmbStatus.SelectedIndex].ToString()).ToList();

                    if (numSum != null && numSum.Value > 0)
                        orders = orders.Where(o => o.TotalAmount >= numSum.Value).ToList();

                    return orders
                        .Select(o => new
                        {
                            o.OrderID,
                            Машина = o.Car != null ? $"{o.Car.Brand} {o.Car.Model}" : "N/A",
                            o.Status,
                            o.TotalAmount,
                            o.DateReceived
                        })
                        .ToList<object>();

                case "Клиенты":
                    var cmbType = pnlFilters.Controls.OfType<ComboBox>().FirstOrDefault();

                    var clients = context.Clients.ToList();

                    if (cmbType?.SelectedIndex > 0)
                        clients = clients.Where(c => c.ClientType == cmbType.Items[cmbType.SelectedIndex].ToString()).ToList();

                    return clients
                        .Select(c => new { c.ClientID, c.FullName, c.Phone, c.Email, c.ClientType })
                        .ToList<object>();

                case "Автомобили":
                    var cmbBrand = pnlFilters.Controls.OfType<ComboBox>().FirstOrDefault();

                    var cars = context.Cars.Include(c => c.Client).ToList();

                    if (cmbBrand?.SelectedIndex > 0)
                        cars = cars.Where(c => c.Brand == cmbBrand.Items[cmbBrand.SelectedIndex].ToString()).ToList();

                    return cars
                        .Select(c => new { c.CarID, c.Brand, c.Model, c.Year, c.VIN, Владелец = c.Client?.FullName ?? "N/A" })
                        .ToList<object>();

                case "Сотрудники":
                    var cmbPos = pnlFilters.Controls.OfType<ComboBox>().FirstOrDefault();
                    var numSalary = pnlFilters.Controls.OfType<NumericUpDown>().FirstOrDefault();

                    var employees = context.Employees.ToList();

                    if (cmbPos?.SelectedIndex > 0)
                        employees = employees.Where(e => e.Position == cmbPos.Items[cmbPos.SelectedIndex].ToString()).ToList();

                    if (numSalary != null && numSalary.Value > 0)
                        employees = employees.Where(e => e.Salary >= numSalary.Value).ToList();

                    return employees
                        .Select(e => new { e.EmployeeID, e.FullName, e.Position, e.Phone, e.Salary })
                        .ToList<object>();

                case "Запчасти":
                    var cmbMfr = pnlFilters.Controls.OfType<ComboBox>().FirstOrDefault();

                    var parts = context.Parts.ToList();

                    if (cmbMfr?.SelectedIndex > 0)
                        parts = parts.Where(p => p.Manufacturer == cmbMfr.Items[cmbMfr.SelectedIndex].ToString()).ToList();

                    return parts
                        .Select(p => new { p.PartID, p.PartName, p.PartNumber, p.Manufacturer, p.Price })
                        .ToList<object>();

                case "Услуги":
                    var cmbSvc = pnlFilters.Controls.OfType<ComboBox>().FirstOrDefault();

                    var services = context.Services.ToList();

                    if (cmbSvc?.SelectedIndex > 0)
                        services = services.Where(s => s.ServiceName == cmbSvc.Items[cmbSvc.SelectedIndex].ToString()).ToList();

                    return services
                        .Select(s => new { s.ServiceID, s.ServiceName, s.Description, s.Price })
                        .ToList<object>();

                default:
                    return new List<object>();
            }
        }

        private void ExportCsv()
        {
            if (currentData == null || currentData.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта");
                return;
            }

            var sfd = new SaveFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv",
                DefaultExt = "csv",
                FileName = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sb = new StringBuilder();
                    var first = currentData.First();
                    var props = first.GetType().GetProperties();

                    // Заголовок
                    sb.AppendLine(string.Join(",", props.Select(p => p.Name)));

                    // Данные
                    foreach (var item in currentData)
                    {
                        var values = props.Select(p =>
                        {
                            var val = p.GetValue(item)?.ToString() ?? "";
                            return $"\"{val}\"";
                        });
                        sb.AppendLine(string.Join(",", values));
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show($"Экспортировано: {sfd.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void ExportJson()
        {
            if (currentData == null || currentData.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта");
                return;
            }

            var sfd = new SaveFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json",
                DefaultExt = "json",
                FileName = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(currentData, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(sfd.FileName, json, Encoding.UTF8);
                    MessageBox.Show($"Экспортировано: {sfd.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }
    }
}
