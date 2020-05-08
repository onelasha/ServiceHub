using System;

namespace ServiceHub
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureCC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureCC / 0.5556);

        public string Summary { get; set; }
    }
}
