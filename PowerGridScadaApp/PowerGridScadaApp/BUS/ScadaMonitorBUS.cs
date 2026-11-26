using System;
using PowerGridScadaApp.DAL;
using PowerGridScadaApp.DTO;

namespace PowerGridScadaApp.BUS
{
    public class ScadaMonitorBUS
    {
        private GridLogDAL dal = new GridLogDAL();
        private Random rnd = new Random();

        public GridTelemetry ThuThapGoiTinScada(string stationName)
        {
            double voltage = Math.Round(108.0 + rnd.NextDouble() * 7.0, 2);
            double load = Math.Round(50.0 + rnd.NextDouble() * 48.0, 1);

            GridTelemetry telemetry = new GridTelemetry
            {
                StationName = stationName,
                Timestamp = DateTime.Now,
                VoltageKV = voltage,
                LoadPercent = load
            };

            if (telemetry.IsOverload) dal.InsertOverloadIncident(telemetry);
            return telemetry;
        }
    }
}