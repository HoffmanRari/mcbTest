using CsvHelper;
using CsvHelper.Configuration;
using MCB.DataAccess;
using OfficeOpenXml;
using System.Globalization;

namespace MCB.Service
{
    public class DataService
    {
        private readonly DataRepository _dataRepository;
        private readonly string _corruptionDataPath;
        private readonly string _developmentDataPath;

        public DataService(string corruptionDataPath, string developmentDataPath)
        {
            _dataRepository = new DataRepository();
            _corruptionDataPath = corruptionDataPath;
            _developmentDataPath = developmentDataPath;
        }

        public void ProcessData()
        {
            // Fetch data
            //  var corruptionData = FetchCorruptionData();
            var developmentData = FetchDevelopmentData();

            // Transform data
            //  var transformedCorruptionData = TransformCorruptionData(corruptionData);
            var transformedDevelopmentData = TransformDevelopmentData(developmentData);

            // Load data
            // _dataRepository.LoadOrUpdateCorruptionData(transformedCorruptionData);
            _dataRepository.LoadOrUpdateDevelopmentData(transformedDevelopmentData);
        }

        private List<Dictionary<string, object>> FetchDataFromXLsx(string path)
        {
            var data = new List<Dictionary<string, object>>();

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int startRow = worksheet.Dimension.Start.Row;
                int endRow = worksheet.Dimension.End.Row;
                int startColumn = worksheet.Dimension.Start.Column;
                int endColumn = worksheet.Dimension.End.Column;

                // Get headers from the first row
                var headers = new List<string>();
                for (int col = startColumn; col <= endColumn; col++)
                {
                    headers.Add(worksheet.Cells[startRow, col].Text);
                }

                // Read each row
                for (int row = startRow + 1; row <= endRow; row++)
                {
                    var record = new Dictionary<string, object>();
                    for (int col = startColumn; col <= endColumn; col++)
                    {
                        var header = headers[col - startColumn];
                        var value = worksheet.Cells[row, col].Text;

                        // Handle null or empty values by replacing them with an empty dictionary
                        record[header] = !string.IsNullOrEmpty(value) ? (object)value : new Dictionary<string, object>();
                    }
                    data.Add(record);
                }
            }

            return data;
        }

        private List<Dictionary<string, object>> FetchCorruptionData()
        {
            return FetchDataFromXLsx(_corruptionDataPath);
        }

        public IEnumerable<Dictionary<string, object>> FetchDevelopmentData()
        {
            return FetchDataFromXLsx(_developmentDataPath);
        }


        private IEnumerable<Dictionary<string, object>> FetchDevelopmentDataa()
        {
            using (var reader = new StreamReader(_developmentDataPath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                var records = new List<Dictionary<string, object>>();
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = new Dictionary<string, object>();
                    if (csv.Context?.Reader?.HeaderRecord == null)
                    {
                        continue;
                    }

                    foreach (var header in csv.Context.Reader.HeaderRecord)
                    {
                        var fieldValue = csv.GetField(header);

                        // Handle null values by replacing them with an empty dictionary
                        record[header] = !string.IsNullOrEmpty(fieldValue) ? (object)fieldValue : new Dictionary<string, object>();
                    }

                    records.Add(record);
                }

                return records;
            }
        }


        public IEnumerable<CorruptionData> TransformCorruptionData(List<Dictionary<string, object>> data, int yearB = 2012, int yearEnd = 2020)
        {
            var transformedData = new List<CorruptionData>();

            foreach (var row in data)
            {
                string countryName = row["country"].ToString();
                string iso3 = row["iso3"].ToString();
                string region = row["region"].ToString();

                for (int year = yearEnd; year >= yearB; year--)
                {
                    if (double.TryParse(row[$"CPI score {year}"].ToString(), out double cpiScore) &&
                        int.TryParse(row[$"Rank {year}"].ToString(), out int rank) &&
                        int.TryParse(row[$"Sources {year}"].ToString(), out int sources) &&
                        double.TryParse(row[$"Standard error {year}"].ToString(), out double standardError))
                    {
                        transformedData.Add(new CorruptionData
                        {
                            CountryName = countryName,
                            Iso3 = iso3,
                            Region = region,
                            Year = year,
                            CpiScore = cpiScore,
                            Rank = rank,
                            Sources = sources,
                            StandardError = standardError
                        });
                    }
                }
            }

            return transformedData;
        }

        public IEnumerable<DevelopmentData> TransformDevelopmentData(IEnumerable<Dictionary<string, object>> data)
        {
            var transformedData = new List<DevelopmentData>();
            foreach (var record in data)
            {
                string countryName = record["Country Name"].ToString();
                string countryCode = record["Country Code"].ToString();
                string indicatorName = record["Indicator Name"].ToString();
                string indicatorCode = record["Indicator Code"].ToString();

                for (int year = 1960; year <= 2023; year++)
                {
                    if (record.ContainsKey(year.ToString()) && double.TryParse(record[year.ToString()]?.ToString(), out double indicatorValue))
                    {
                        transformedData.Add(new DevelopmentData
                        {
                            CountryName = countryName,
                            CountryCode = countryCode,
                            IndicatorName = indicatorName,
                            IndicatorCode = indicatorCode,
                            Year = year,
                            IndicatorValue = indicatorValue
                        });
                    }
                }
            }
            return transformedData;
        }

    }
}
