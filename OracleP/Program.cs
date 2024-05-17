

using CsvHelper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Globalization;

class Program
{
    static void Main()
    {
        string cpiFilePath = @"C:\\app\\CorruptionPerceptionIndexDataSet.csv";
        string wdiFilePath = @"C:\app\WDICSV.csv";

        var cpiData = ExtractData(cpiFilePath);
        var wdiData = ExtractData(wdiFilePath);

        var transformedCpiData = TransformCpiData(cpiData);
        var transformedWdiData = TransformWdiData(wdiData);

        LoadData(transformedCpiData, transformedWdiData);
    }

    static DataTable ExtractData(string filePath)
    {
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            using (var dr = new CsvDataReader(csv))
            {
                var dt = new DataTable();
                dt.Load(dr);
                return dt;
            }
        }
    }

    static DataTable TransformCpiData(DataTable cpiData)
    {
        // Columns to keep and pivot
        string[] columnsToKeep = { "Country" };
        string[] columnsToPivot = { "CPI Score 2012", "CPI Score 2013", "CPI Score 2014", "CPI Score 2015", "CPI Score 2016", "CPI Score 2017", "CPI Score 2018", "CPI Score 2019", "CPI Score 2020" };

        // Create a new DataTable to hold the transformed data
        DataTable transformedTable = new DataTable();
        transformedTable.Columns.Add("country", typeof(string));
        transformedTable.Columns.Add("year", typeof(int));
        transformedTable.Columns.Add("cpi_score", typeof(double));

        // Iterate through each row in the original DataTable
        foreach (DataRow row in cpiData.Rows)
        {
            string country = row["Country"].ToString();

            // Pivot the CPI score columns
            for (int year = 2012; year <= 2020; year++)
            {
                string columnName = $"CPI Score {year}";
                if (row.Table.Columns.Contains(columnName) && !row.IsNull(columnName))
                {
                    DataRow newRow = transformedTable.NewRow();
                    newRow["country"] = country;
                    newRow["year"] = year;
                    newRow["cpi_score"] = Convert.ToDouble(row[columnName]);
                    transformedTable.Rows.Add(newRow);
                }
            }
        }

        // Handle missing values by removing rows with null CPI scores
        transformedTable = transformedTable.AsEnumerable()
                                           .Where(r => r.Field<double?>("cpi_score").HasValue)
                                           .CopyToDataTable();

        return transformedTable;
    }

    static DataTable TransformWdiData(DataTable wdiData)
    {
        var indicators = new string[]
        {
            "Total fisheries production (metric tons)",
            "Agricultural land (sq. km)",
            "Time required to start a business (days)",
            "New businesses registered (number)",
            "Employment in agriculture (% of total employment) (modeled ILO estimate)",
            "Self-employed, total (% of total employment) (modeled ILO estimate)"
        };

        // Create a new DataTable to hold the transformed data
        DataTable transformedTable = new DataTable();
        transformedTable.Columns.Add("country", typeof(string));
        transformedTable.Columns.Add("indicator", typeof(string));
        transformedTable.Columns.Add("year", typeof(int));
        transformedTable.Columns.Add("value", typeof(double));

        // Iterate through each row in the original DataTable
        foreach (DataRow row in wdiData.Rows)
        {
            string country = row["Country Name"].ToString();
            string indicator = row["Indicator Name"].ToString();

            if (indicators.Contains(indicator))
            {
                // Pivot the year columns
                for (int year = 1960; year <= 2020; year++)
                {
                    string columnName = year.ToString();
                    if (row.Table.Columns.Contains(columnName) && !row.IsNull(columnName))
                    {
                        DataRow newRow = transformedTable.NewRow();
                        newRow["country"] = country;
                        newRow["indicator"] = indicator;
                        newRow["year"] = year;
                        newRow["value"] = Convert.ToDouble(row[columnName]);
                        transformedTable.Rows.Add(newRow);
                    }
                }
            }
        }

        // Handle missing values by removing rows with null values
        transformedTable = transformedTable.AsEnumerable()
                                           .Where(r => r.Field<double?>("value").HasValue)
                                           .CopyToDataTable();

        return transformedTable;
    }

    static void LoadData(DataTable cpiData, DataTable wdiData)
    {
        string connString = "User Id=Hoffman;Password=Hoffman24!;Data Source=localhost:1521/XEPDB1";
        using (var conn = new OracleConnection(connString))
        {
            conn.Open();

            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    foreach (DataRow row in cpiData.Rows)
                    {
                        using (var cmd = new OracleCommand("data_pipeline_pkg.load_corruption_perception_index", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("p_country", OracleDbType.Varchar2).Value = row["country"];
                            cmd.Parameters.Add("p_year", OracleDbType.Int32).Value = Convert.ToInt32(row["year"]);
                            cmd.Parameters.Add("p_cpi_score", OracleDbType.Double).Value = Convert.ToDouble(row["cpi_score"]);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    foreach (DataRow row in wdiData.Rows)
                    {
                        using (var cmd = new OracleCommand("data_pipeline_pkg.load_world_development_indicators", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("p_country", OracleDbType.Varchar2).Value = row["country"];
                            cmd.Parameters.Add("p_indicator", OracleDbType.Varchar2).Value = row["indicator"];
                            cmd.Parameters.Add("p_year", OracleDbType.Int32).Value = Convert.ToInt32(row["year"]);
                            cmd.Parameters.Add("p_value", OracleDbType.Double).Value = Convert.ToDouble(row["value"]);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
