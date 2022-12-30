namespace Dax.Model.Extractor.Tests
{
    using Dax.Metadata.Extractor;
    using Microsoft.AnalysisServices;
    using System;
    using System.Linq;
    using Xunit;

    public class TomExtractorTests : IClassFixture<ModelFixture>
    {
        private readonly ModelFixture _fixture;

        public TomExtractorTests(ModelFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void GetDaxModel_Populate_ContosoTest()
        {
            var daxModel = TomExtractor.GetDaxModel(_fixture.Contoso, extractorApp: "VPAXUnitTest", extractorVersion: "1.0.0.0");

            Assert.Equal(10, daxModel.Tables.Count);
            Assert.Equal(10, daxModel.Relationships.Count);
            Assert.Single(daxModel.Roles);

            Assert.Equal(1550, daxModel.CompatibilityLevel);
            Assert.Equal(CompatibilityMode.Unknown.ToString(), daxModel.CompatibilityMode);
            Assert.Equal(DateTime.MinValue, daxModel.LastProcessed);
            Assert.Equal(DateTime.MinValue, daxModel.LastUpdate);
            Assert.Equal(0, daxModel.Version);
            Assert.True((DateTime.UtcNow - daxModel.ExtractionDate) < TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void GetDaxModel_Populate_TestModel1Test()
        {
            var daxModel = TomExtractor.GetDaxModel(_fixture.TestModel1, extractorApp: "VPAXUnitTest", extractorVersion: "1.0.0.0");

            Assert.Single(daxModel.Tables);
            Assert.Empty(daxModel.Relationships);
            Assert.Empty(daxModel.Roles);

            Assert.Equal(1550, daxModel.CompatibilityLevel);
            Assert.Equal(CompatibilityMode.Unknown.ToString(), daxModel.CompatibilityMode);
        }

        [Fact]
        public void GetDaxModel_ColumnProperty_IsHidden_Test()
        {
            var daxModel = TomExtractor.GetDaxModel(_fixture.TestModel1, extractorApp: "VPAXUnitTest", extractorVersion: "1.0.0.0");

            var table = daxModel.Tables.SingleOrDefault((t) => t.TableName.Name == "CalculatedTable1");
            Assert.NotNull(table);

            var column = table.Columns.SingleOrDefault((c) => c.ColumnName.Name == "Name-IsHidden");
            Assert.NotNull(column);
            Assert.True(column.IsHidden);
        }

        [Fact]
        public void GetDaxModel_ColumnProperty_SortByColumn_Test()
        {
            var daxModel = TomExtractor.GetDaxModel(_fixture.TestModel1, extractorApp: "VPAXUnitTest", extractorVersion: "1.0.0.0");

            var table = daxModel.Tables.SingleOrDefault((t) => t.TableName.Name == "CalculatedTable1");
            Assert.NotNull(table);

            var column = table.Columns.SingleOrDefault((c) => c.ColumnName.Name == "Name-SortByColumn");
            Assert.NotNull(column);
            Assert.Equal("Index", column.SortByColumnName.Name);
        }

        [Fact]
        public void GetDaxModel_ColumnProperty_GroupByColumns_Test()
        {
            var daxModel = TomExtractor.GetDaxModel(_fixture.TestModel1, extractorApp: "VPAXUnitTest", extractorVersion: "1.0.0.0");

            var table = daxModel.Tables.SingleOrDefault((t) => t.TableName.Name == "CalculatedTable1");
            Assert.NotNull(table);

            var column = table.Columns.SingleOrDefault((c) => c.ColumnName.Name == "Name-GroupByColumns");
            Assert.NotNull(column);
            Assert.Single(column.GroupByColumns);

            var groupingColumn = column.GroupByColumns[0];
            Assert.Equal("Index", groupingColumn.Name);
        }
    }
}