using System.Data;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace ECommerceApp.Helpers
{
    public class OracleDbHelper
    {
        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings["OracleConn"].ConnectionString;

        public static OracleConnection GetConnection()
        {
            return new OracleConnection(connectionString);
        }

        public static DataTable ExecuteQuery(string query, OracleParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = GetConnection())
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        da.Fill(dt);
                }
            }
            return dt;
        }

        public static int ExecuteNonQuery(string query, OracleParameter[] parameters = null)
        {
            using (OracleConnection conn = GetConnection())
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string query, OracleParameter[] parameters = null)
        {
            using (OracleConnection conn = GetConnection())
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
