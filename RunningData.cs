using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TemperatureAnalyzer
{
    public class RunningData
    {
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public double DistanceKm { get; set; }
        public int DurationMinutes { get; set; }
        public double AvgSpeedKmh { get; set; }
        public double MaxSpeedKmh { get; set; }
        public double MinSpeedKmh { get; set; }
        public int AvgPulse { get; set; }

        public static List<RunningData> LoadFromCsv(string filePath)
        {
            var list = new List<RunningData>();
            var lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);

            int startLine = 0;
            while (startLine < lines.Length && string.IsNullOrWhiteSpace(lines[startLine]))
                startLine++;

            if (startLine >= lines.Length) return list;
            startLine++;

            CultureInfo invCult = CultureInfo.InvariantCulture;

            for (int i = startLine; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                char separator = line.Contains(';') ? ';' : ',';
                var parts = line.Split(separator);

                if (parts.Length < 7) continue;

                try
                {
                    for (int j = 0; j < parts.Length; j++)
                        parts[j] = parts[j].Trim();

                    var data = new RunningData
                    {
                        Day = i - startLine + 1,
                        Date = DateTime.ParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        DistanceKm = double.Parse(parts[1], invCult),
                        DurationMinutes = int.Parse(parts[2]),
                        AvgSpeedKmh = double.Parse(parts[3], invCult),
                        MaxSpeedKmh = double.Parse(parts[4], invCult),
                        MinSpeedKmh = double.Parse(parts[5], invCult),
                        AvgPulse = int.Parse(parts[6])
                    };
                    list.Add(data);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка в строке {i + 1}: {ex.Message}");
                }
            }
            return list;
        }

        public static double GetWeekendTotalKm(List<RunningData> data)
        {
            return data.Where(d => d.Date.DayOfWeek == DayOfWeek.Saturday ||
                                   d.Date.DayOfWeek == DayOfWeek.Sunday)
                       .Sum(d => d.DistanceKm);
        }
    }
}