using MCB.DataAccess.Database;
using System.Data;
using System.Data.SqlClient;

namespace MCB.DataAccess
{
    public class DataRepository
    {
        private readonly string _connectionString;

        public DataRepository()
        {
            _connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=macroeconomic_analysis ;Integrated Security=True";
        }

        public void LoadOrUpdateCorruptionData(IEnumerable<CorruptionData> corruptionData)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (var data in corruptionData)
                {
                    using (var command = new SqlCommand(StoredProcedure.CorruptionData_Upsert, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@CountryName", data.CountryName);
                        command.Parameters.AddWithValue("@Iso3", data.Iso3);
                        command.Parameters.AddWithValue("@Region", data.Region);
                        command.Parameters.AddWithValue("@Year", data.Year);
                        command.Parameters.AddWithValue("@CpiScore", data.CpiScore);
                        command.Parameters.AddWithValue("@Rank", data.Rank);
                        command.Parameters.AddWithValue("@Sources", data.Sources);
                        command.Parameters.AddWithValue("@StandardError", data.StandardError);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void LoadOrUpdateDevelopmentData(IEnumerable<DevelopmentData> developmentData)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (var data in developmentData)
                {
                    using (var command = new SqlCommand(StoredProcedure.DevelopmentData_Upsert, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@CountryName", data.CountryName);
                        command.Parameters.AddWithValue("@CountryCode", data.CountryCode);
                        command.Parameters.AddWithValue("@IndicatorName", data.IndicatorName);
                        command.Parameters.AddWithValue("@IndicatorCode", data.IndicatorCode);
                        command.Parameters.AddWithValue("@Year", data.Year);
                        command.Parameters.AddWithValue("@IndicatorValue", data.IndicatorValue);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
