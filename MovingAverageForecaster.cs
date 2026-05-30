using System;
using System.Collections.Generic;
using System.Linq;

namespace TemperatureAnalyzer
{
    public interface IForecaster
    {
        List<double> Forecast(List<double> historical, int windowSize, int forecastDays);
    }

    public class MovingAverageForecaster : IForecaster
    {
        public List<double> Forecast(List<double> historical, int windowSize, int forecastDays)
        {
            if (historical == null || historical.Count < windowSize)
                throw new ArgumentException("Недостаточно данных для указанного периода");

            var forecast = new List<double>();
            var currentSeries = new List<double>(historical);

            for (int i = 0; i < forecastDays; i++)
            {
                double avg = currentSeries.Skip(currentSeries.Count - windowSize).Take(windowSize).Average();
                forecast.Add(avg);
                currentSeries.Add(avg); // используем прогноз для следующего шага
            }
            return forecast;
        }
    }
}