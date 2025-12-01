using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace Autoservice.Services
{
    public static class ExportService
    {
        public static void ExportToCsv(IEnumerable<object> data, string filePath)
        {
            if (data == null || !data.Any())
            {
                throw new ArgumentException("Нет данных для экспорта");
            }

            var sb = new StringBuilder();
            var firstItem = data.First();
            var properties = firstItem.GetType().GetProperties();

            // Заголовки
            sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Данные
            foreach (var item in data)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item, null);
                    if (value == null) return "";
                    string str = value.ToString();
                    if (str.Contains(",") || str.Contains("\""))
                    {
                        str = "\"" + str.Replace("\"", "\"\"") + "\"";
                    }
                    return str;
                });
                sb.AppendLine(string.Join(",", values));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        public static void ExportToJson(IEnumerable<object> data, string filePath)
        {
            if (data == null || !data.Any())
            {
                throw new ArgumentException("Нет данных для экспорта");
            }

            string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }
    }
}
