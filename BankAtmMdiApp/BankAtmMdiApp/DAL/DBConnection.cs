using System.Configuration;
using System.Data.SqlClient;

namespace BankAtmMdiApp.DAL
{
    public class DBConnection
    {
        public static SqlConnection GetConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            return new SqlConnection(connStr);
        }
    }
}