namespace Dax.Model.Extractor.Tests
{
    using Microsoft.AnalysisServices;
    using System;
    using Xunit;
    using TOM = Microsoft.AnalysisServices.Tabular;

    public class TomExtractorTests : IClassFixture<TomExtractorFixture>
    {
        private readonly TomExtractorFixture _fixture;

        public TomExtractorTests(TomExtractorFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("VPAXUnitTest", "VPAXUnitTest")]
        public void GetDaxModel_ExtractorApp_Test(string extractorApp, string expected)
        {
            var daxModel = TomExtractor.GetDaxModel(_fixture.Contoso, extractorApp, extractorVersion: null);
            Assert.Equal(expected, daxModel.ExtractorApp);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("1.2.3.4", "1.2.3.4")]
        public void GetDaxModel_ExtractorVersion_Test(string extractorVersion, string expected)
        {
            var daxModel = TomExtractor.GetDaxModel(_fixture.Contoso, extractorApp: null, extractorVersion);
            Assert.Equal(expected, daxModel.ExtractorAppVersion);
        }

        [Fact]
        public void GetDaxModel_FromTomModel_Contoso_Test()
        {
            var daxModel = GetDaxModelFromTomModel(_fixture.Contoso);

            Assert.Equal("en-US", daxModel.Culture);
            Assert.Equal(1550, daxModel.CompatibilityLevel);
            Assert.Equal(CompatibilityMode.Unknown.ToString(), daxModel.CompatibilityMode);
            Assert.Equal(DateTimeOffset.Parse("2022-03-07T16:33:34.6033330+01:00"), daxModel.LastProcessed);
            Assert.Equal(DateTimeOffset.Parse("2023-03-28T15:41:29.3433330+02:00"), daxModel.LastUpdate);
            Assert.Equal(0, daxModel.Version);
            Assert.True((DateTime.UtcNow - daxModel.ExtractionDate) < TimeSpan.FromSeconds(10));

            Assert.Equal(10, daxModel.Tables.Count);
            Assert.Equal(10, daxModel.Relationships.Count);
            Assert.Single(daxModel.Roles);
        }

        [Fact]
        public void GetDaxModel_FromTomModel_Vaccini_Test()
        {
            var daxModel = GetDaxModelFromTomModel(_fixture.Vaccini);

            Assert.Equal("it-IT", daxModel.Culture);
            Assert.Equal(1567, daxModel.CompatibilityLevel);
            Assert.Equal(CompatibilityMode.Unknown.ToString(), daxModel.CompatibilityMode);
            Assert.Equal(DateTimeOffset.Parse("2021-04-20T17:58:07.836667+02:00"), daxModel.LastProcessed);
            Assert.Equal(DateTimeOffset.Parse("2023-03-28T15:46:54.613333+02:00"), daxModel.LastUpdate);
            Assert.Equal(0, daxModel.Version);
            Assert.True((DateTime.UtcNow - daxModel.ExtractionDate) < TimeSpan.FromSeconds(10));

            Assert.Equal(13, daxModel.Tables.Count);
            Assert.Equal(22, daxModel.Relationships.Count);
            Assert.Empty(daxModel.Roles);
        }

        private Metadata.Model GetDaxModelFromTomModel(TOM.Model tomModel)
        {
            var daxModel = TomExtractor.GetDaxModel(tomModel, extractorApp: "VPAXUnitTest", extractorVersion: "1.0.0.0");
            return daxModel;
        }
    }
}