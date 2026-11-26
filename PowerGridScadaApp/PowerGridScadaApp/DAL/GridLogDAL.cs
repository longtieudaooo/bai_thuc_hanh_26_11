using System.Data.SqlClient;
using PowerGridScadaApp.DTO;

namespace PowerGridScadaApp.DAL
{
    public class GridLogDAL
    {
        public void InsertOverloadIncident(GridTelemetry data)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = @"INSERT INTO GridIncidentLogs (StationName, RecordedTime, VoltageKV, LoadPercent, AlertMessage)
                               VALUES (@Station, @Time, @Volt, @Load, @Msg)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Station", data.StationName);
                cmd.Parameters.AddWithValue("@Time", data.Timestamp);
                cmd.Parameters.AddWithValue("@Volt", data.VoltageKV);
                cmd.Parameters.AddWithValue("@Load", data.LoadPercent);
                cmd.Parameters.AddWithValue("@Msg", $"CẢNH BÁO QUÁ TẢI: Mức mang tải đạt {data.LoadPercent:F1}% (> 85%)");
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}