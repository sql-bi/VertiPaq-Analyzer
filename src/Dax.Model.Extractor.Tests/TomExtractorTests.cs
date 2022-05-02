namespace Dax.Model.Extractor.Tests
{
    using Dax.Metadata.Extractor;
    using Microsoft.AnalysisServices;
    using System;
    using System.IO;
    using Xunit;
    using TOM = Microsoft.AnalysisServices.Tabular;

    public class TomExtractorTests
    {
        private const string ContosoBimPath = @".\_data\bim\Contoso.bim";

        [Fact]
        public void GetDaxModel_PopulateTest()
        {
            var tomModel = GetTestTomModel();
            var daxModel = TomExtractor.GetDaxModel(tomModel, extractorApp: "VPAXUnitTest", extractorVersion: "1.0.0.0");

            Assert.Equal(10, daxModel.Tables.Count);
            Assert.Equal(10, daxModel.Relationships.Count);
            Assert.Single(daxModel.Roles);

            Assert.Equal(1550, daxModel.CompatibilityLevel);
            Assert.NotNull(daxModel.CompatibilityMode);
            Assert.Equal(DateTime.MinValue, daxModel.LastProcessed);
            Assert.Equal(DateTime.MinValue, daxModel.LastUpdate);
            Assert.Equal(0, daxModel.Version);
            Assert.True((DateTime.UtcNow - daxModel.ExtractionDate) < TimeSpan.FromSeconds(10));
        }

        private TOM.Model GetTestTomModel()
        {
            var database = TOM.JsonSerializer.DeserializeDatabase(File.ReadAllText(ContosoBimPath));
            return database.Model;
        }
    }
}