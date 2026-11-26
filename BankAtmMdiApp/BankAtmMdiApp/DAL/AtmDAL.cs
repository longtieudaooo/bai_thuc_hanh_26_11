using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BankAtmMdiApp.DTO;

namespace BankAtmMdiApp.DAL
{
    public class AtmDAL
    {
        public List<AtmStation> GetAllStations()
        {
            List<AtmStation> list = new List<AtmStation>();
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                conn.Open(); // <--- Thêm dòng này
                SqlCommand cmd = new SqlCommand("SELECT * FROM AtmStations", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new AtmStation
                    {
                        StationID = reader["StationID"].ToString(),
                        Address = reader["Address"].ToString(),
                        CashBalance = Convert.ToDecimal(reader["CashBalance"]),
                        Status = reader["Status"].ToString(),
                        ImagePath = reader["ImagePath"] == DBNull.Value ? "" : reader["ImagePath"].ToString()
                    });
                }
            }
            return list;
        }

        public bool InsertOrUpdateStation(AtmStation atm)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = @"
                    IF EXISTS (SELECT 1 FROM AtmStations WHERE StationID = @ID)
                        UPDATE AtmStations SET Address = @Addr, CashBalance = @Cash, Status = @Status, ImagePath = @Img WHERE StationID = @ID
                    ELSE
                        INSERT INTO AtmStations (StationID, Address, CashBalance, Status, ImagePath) VALUES (@ID, @Addr, @Cash, @Status, @Img)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", atm.StationID);
                cmd.Parameters.AddWithValue("@Addr", atm.Address);
                cmd.Parameters.AddWithValue("@Cash", atm.CashBalance);
                cmd.Parameters.AddWithValue("@Status", atm.Status);
                cmd.Parameters.AddWithValue("@Img", string.IsNullOrEmpty(atm.ImagePath) ? (object)DBNull.Value : atm.ImagePath);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SaveReconcileLog(ReconcileResult res)
        {
            using (SqlConnection conn = DBConnection.GetConnection())
            {
                string sql = @"INSERT INTO ReconciliationLogs (ReconcileDate, TotalTransactions, MatchedCount, DiscrepancyCount, Note)
                               VALUES (@Date, @Total, @Matched, @Diff, @Note)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Date", res.ReconcileDate);
                cmd.Parameters.AddWithValue("@Total", res.TotalTransactions);
                cmd.Parameters.AddWithValue("@Matched", res.MatchedCount);
                cmd.Parameters.AddWithValue("@Diff", res.DiscrepancyCount);
                cmd.Parameters.AddWithValue("@Note", res.Note);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}