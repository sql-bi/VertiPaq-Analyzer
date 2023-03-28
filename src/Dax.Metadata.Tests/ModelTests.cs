namespace Dax.Metadata.Tests
{
    using System;
    using Xunit;

    public class ModelTests : IClassFixture<DaxMetadataFixture>
    {
        private readonly DaxMetadataFixture _fixture;

        public ModelTests(DaxMetadataFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void DaxModelVersion_DeserializeFromVpaxTest()
        {
            Assert.Equal("1.2.0", _fixture.Contoso.DaxModelVersion);
            Assert.Equal("1.2.0", _fixture.Vaccini.DaxModelVersion);
        }

        [Fact]
        public void ExtractorApp_DeserializeFromVpaxTest()
        {
            Assert.Equal("Bravo", _fixture.Contoso.ExtractorApp);
            Assert.Equal("Bravo", _fixture.Vaccini.ExtractorApp);
        }

        [Fact]
        public void ExtractorAppVersion_DeserializeFromVpaxTest()
        {
            Assert.Equal("1.0.2", _fixture.Contoso.ExtractorAppVersion);
            Assert.Equal("1.0.2", _fixture.Vaccini.ExtractorAppVersion);
        }

        [Fact]
        public void ExtractorLib_DeserializeFromVpaxTest()
        {
            Assert.Equal("Dax.Model.Extractor", _fixture.Contoso.ExtractorLib);
            Assert.Equal("Dax.Model.Extractor", _fixture.Vaccini.ExtractorLib);
        }

        [Fact]
        public void ExtractorLibVersion_DeserializeFromVpaxTest()
        {
            Assert.Equal("1.2.11+12cc1db610901cc98fbe76c9907f0114a2b62fb4", _fixture.Contoso.ExtractorLibVersion);
            Assert.Equal("1.2.11+12cc1db610901cc98fbe76c9907f0114a2b62fb4", _fixture.Vaccini.ExtractorLibVersion);
        }

        [Fact]
        public void DaxModelLib_DeserializeFromVpaxTest()
        {
            Assert.Equal("Dax.Metadata", _fixture.Contoso.DaxModelLib);
            Assert.Equal("Dax.Metadata", _fixture.Vaccini.DaxModelLib);
        }

        [Fact]
        public void DaxModelLibVersion_DeserializeFromVpaxTest()
        {
            Assert.Equal("1.2.11+12cc1db610901cc98fbe76c9907f0114a2b62fb4", _fixture.Contoso.DaxModelLibVersion);
            Assert.Equal("1.2.11+12cc1db610901cc98fbe76c9907f0114a2b62fb4", _fixture.Vaccini.DaxModelLibVersion);
        }

        [Fact]
        public void ServerName_DeserializeFromVpaxTest()
        {
            Assert.Equal("LAPTOP-12C9A7VU\\AnalysisServicesWorkspace_f4d52542-7374-42d9-b055-9dcb900e997a", _fixture.Contoso.ServerName.Name);
            Assert.Equal("LAPTOP-12C9A7VU\\AnalysisServicesWorkspace_1086f839-1e3b-44e6-8da0-fe20013b852b", _fixture.Vaccini.ServerName.Name);
        }

        [Fact]
        public void ModelName_DeserializeFromVpaxTest()
        {
            Assert.Equal("fda6fc7a-0b7d-4a44-b8d0-59433669f678", _fixture.Contoso.ModelName.Name);
            Assert.Equal("d3216176-89dc-4cdb-8d3d-797be166a61a", _fixture.Vaccini.ModelName.Name);
        }

        [Fact]
        public void CompatibilityLevel_DeserializeFromVpaxTest()
        {
            Assert.Equal(1550, _fixture.Contoso.CompatibilityLevel);
            Assert.Equal(1567, _fixture.Vaccini.CompatibilityLevel);
        }

        [Fact]
        public void CompatibilityMode_DeserializeFromVpaxTest()
        {
            Assert.Equal("PowerBI", _fixture.Contoso.CompatibilityMode);
            Assert.Equal("PowerBI", _fixture.Vaccini.CompatibilityMode);
        }

        [Fact]
        public void ExtractionDate_DeserializeFromVpaxTest()
        {
            Assert.Equal(DateTimeOffset.Parse("2023-03-28T13:41:48.9489073Z"), _fixture.Contoso.ExtractionDate);
            Assert.Equal(DateTimeOffset.Parse("2023-03-28T13:47:13.2710617Z"), _fixture.Vaccini.ExtractionDate);
        }

        [Fact]
        public void LastDataRefresh_DeserializeFromVpaxTest()
        {
            Assert.Equal(DateTimeOffset.Parse("2022-05-06T13:21:12.926667Z"), _fixture.Contoso.LastDataRefresh);
            Assert.Equal(DateTimeOffset.Parse("2021-04-20T15:58:07.943333Z"), _fixture.Vaccini.LastDataRefresh);
        }

        [Fact]
        public void LastProcessed_DeserializeFromVpaxTest()
        {
            Assert.Equal(DateTimeOffset.Parse("2022-03-07T16:33:34.603333+01:00"), _fixture.Contoso.LastProcessed);
            Assert.Equal(DateTimeOffset.Parse("2021-04-20T17:58:07.836667+02:00"), _fixture.Vaccini.LastProcessed);
        }

        [Fact]
        public void LastUpdate_DeserializeFromVpaxTest()
        {
            Assert.Equal(DateTimeOffset.Parse("2023-03-28T15:41:29.343333+02:00"), _fixture.Contoso.LastUpdate);
            Assert.Equal(DateTimeOffset.Parse("2023-03-28T15:46:54.613333+02:00"), _fixture.Vaccini.LastUpdate);
        }

        [Fact]
        public void Version_DeserializeFromVpaxTest()
        {
            Assert.Equal(3, _fixture.Contoso.Version);
            Assert.Equal(4, _fixture.Vaccini.Version);
        }
    }
}