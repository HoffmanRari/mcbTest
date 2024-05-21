using MCB.Service;

namespace MCB.Test
{

    [TestClass]
    public class DataServiceTests
    {
        [TestMethod]
        public void TransformCorruptionData_ShouldTransformCorrectly()
        {
            // Arrange
            var corruptionData = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                { "country", "Country1" },
                { "iso3", "C1" },
                { "region", "Region1" },
                { "CPI score 2020", 50.0 },
                { "Rank 2020", 1 },
                { "Sources 2020", 10 },
                { "Standard error 2020", 0.5 },
                { "CPI score 2019", 49.0 },
                { "Rank 2019", 2 },
                { "Sources 2019", 9 },
                { "Standard error 2019", 0.4 }
            }
        };

            var dataService = new DataService(null, null);

            // Act
            var result = dataService.TransformCorruptionData(corruptionData, 2019, 2020).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Country1", result[0].CountryName);
            Assert.AreEqual("C1", result[0].Iso3);
            Assert.AreEqual("Region1", result[0].Region);
            Assert.AreEqual(2020, result[0].Year);
            Assert.AreEqual(50.0, result[0].CpiScore);
            Assert.AreEqual(1, result[0].Rank);
            Assert.AreEqual(10, result[0].Sources);
            Assert.AreEqual(0.5, result[0].StandardError);

            Assert.AreEqual(2019, result[1].Year);
            Assert.AreEqual(49.0, result[1].CpiScore);
            Assert.AreEqual(2, result[1].Rank);
            Assert.AreEqual(9, result[1].Sources);
            Assert.AreEqual(0.4, result[1].StandardError);
        }

        [TestMethod]
        public void TransformDevelopmentData_ShouldTransformCorrectly()
        {
            // Arrange
            var developmentData = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                { "Country Name", "Country1" },
                { "Country Code", "C1" },
                { "Indicator Name", "Indicator1" },
                { "Indicator Code", "I1" },
                { "1960", null },
                { "1961", 1.1 },
                { "2023", 2.3 }
            }
        };

            var dataService = new DataService(null, null);

            // Act
            var result = dataService.TransformDevelopmentData(developmentData).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Country1", result[0].CountryName);
            Assert.AreEqual("C1", result[0].CountryCode);
            Assert.AreEqual("Indicator1", result[0].IndicatorName);
            Assert.AreEqual("I1", result[0].IndicatorCode);
            Assert.AreEqual(1961, result[0].Year);
            Assert.AreEqual(1.1, result[0].IndicatorValue);

            Assert.AreEqual(2023, result[1].Year);
            Assert.AreEqual(2.3, result[1].IndicatorValue);
        }
    }


}
