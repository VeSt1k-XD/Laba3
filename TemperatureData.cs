using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TemperatureAnalyzer
{
    public class TemperatureData
    {
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }
        public double AvgTemp { get; set; }
        public string Description { get; set; }

        public double TempRange => MaxTemp - MinTemp;

        public static List<TemperatureData> LoadFromCsv(string filePath)
        {
            var list = new List<TemperatureData>();
            var lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);

            // Пропускаем пустые строки и ищем первую непустую строку с заголовками
            int startLine = 0;
            while (startLine < lines.Length && string.IsNullOrWhiteSpace(lines[startLine]))
                startLine++;

            if (startLine >= lines.Length) return list;

            // Заголовок (не используем, но пропускаем)
            startLine++;

            CultureInfo invCult = CultureInfo.InvariantCulture; // десятичный разделитель точка

            for (int i = startLine; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Поддерживаем разделители и запятую, и точку с запятой
                char separator = line.Contains(';') ? ';' : ',';
                var parts = line.Split(separator);
                if (parts.Length < 5) continue;

                try
                {
                    // Удаляем лишние пробелы у каждой части
                    for (int j = 0; j < parts.Length; j++)
                        parts[j] = parts[j].Trim();

                    var data = new TemperatureData
                    {
                        Day = i - startLine + 1, // нумерация с 1
                        Date = DateTime.ParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        MaxTemp = double.Parse(parts[1], invCult),
                        MinTemp = double.Parse(parts[2], invCult),
                        AvgTemp = double.Parse(parts[3], invCult),
                        Description = parts[4]
                    };
                    list.Add(data);
                }
                catch (Exception ex)
                {
                    // Можно вывести предупреждение, но продолжим
                    System.Diagnostics.Debug.WriteLine($"Ошибка в строке {i + 1}: {ex.Message}");
                }
            }
            return list;
        }
    }
}