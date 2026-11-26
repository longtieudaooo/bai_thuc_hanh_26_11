using System;

namespace PowerGridScadaApp.DTO
{
    public class GridTelemetry
    {
        public string StationName { get; set; }
        public DateTime Timestamp { get; set; }
        public double VoltageKV { get; set; }
        public double LoadPercent { get; set; }
        public bool IsOverload => LoadPercent > 85.0;
    }
}