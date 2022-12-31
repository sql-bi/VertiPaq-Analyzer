namespace Dax.Model.Extractor.Tests
{
    using Dax.Metadata.Extractor;
    using Microsoft.AnalysisServices;
    using System.Linq;
    using Xunit;
    using TOM = Microsoft.AnalysisServices.Tabular;

    public class DaxModelTests : IClassFixture<DaxModelTestsFixture>
    {
        private readonly DaxModelTestsFixture _fixture;

        public DaxModelTests(DaxModelTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Column_IsHidden_Test()
        {
            var tomModel = CreateTestModel();
            var tomColumn = tomModel.AddTable("T").AddColumn("C");
            
            tomColumn.IsHidden = true;

            var daxModel = GetDaxModel(tomModel);
            var daxColumn = daxModel.GetTable("T").GetColumn("C");

            Assert.True(daxColumn.IsHidden);
        }

        [Fact]
        public void Column_SortByColumn_Test()
        {
            var tomModel = CreateTestModel();
            var tomT1 = tomModel.AddTable("T1");
            var tomC1 = tomT1.AddColumn("C1", TOM.DataType.String);
            var tomC2 = tomT1.AddColumn("C2", TOM.DataType.Int64);

            tomC1.SortByColumn = tomC2;

            var daxModel = GetDaxModel(tomModel);
            var daxC1 = daxModel.GetTable("T1").GetColumn("C1");

            Assert.Equal("C2", daxC1.SortByColumnName.Name);
        }

        [Fact]
        public void Column_GroupByColumns_Test()
        {
            var tomModel = CreateTestModel();
            var tomT1 = tomModel.AddTable("T1");
            var tomC1 = tomT1.AddColumn("C1", TOM.DataType.String);
            var tomC2 = tomT1.AddColumn("C2", TOM.DataType.Int64);
            var tomC3 = tomT1.AddColumn("C3", TOM.DataType.Int64);

            tomC1.AddGroupingColumn(tomC2);
            tomC1.AddGroupingColumn(tomC3);

            var daxModel = GetDaxModel(tomModel);
            var daxC1 = daxModel.GetTable("T1").GetColumn("C1");

            Assert.Equal(2, daxC1.GroupByColumns.Count);
            Assert.Equal("C2", daxC1.GroupByColumns[0].Name);
            Assert.Equal("C3", daxC1.GroupByColumns[1].Name);
        }

        private Metadata.Model GetDaxModel(TOM.Model tomModel)
        {
            var daxModel = TomExtractor.GetDaxModel(tomModel, extractorApp: "VPAXUnitTest", extractorVersion: "1.0.0.0");
            return daxModel;
        }

        private TOM.Model CreateTestModel()
        {
            var database = new TOM.Database(ModelType.Tabular, compatibilityLevel: 1550);
            database.Model = new TOM.Model();
            return database.Model;
        }
    }

    internal static class DaxModelExtensions
    {
        public static Metadata.Table GetTable(this Metadata.Model model, string name)
        {
            var table = model.Tables.Single((t) => t.TableName.Name == name);
            return table;
        }

        public static Metadata.Column GetColumn(this Metadata.Table table, string name)
        {
            var column = table.Columns.Single((c) => c.ColumnName.Name == name);
            return column;
        }
    }

    internal static class TomModelExtensions
    {
        public static TOM.Table AddTable(this TOM.Model model, string name)
        {
            var table = new TOM.Table() {  Name = name };
            model.Tables.Add(table);
            return table;
        }

        public static TOM.DataColumn AddColumn(this TOM.Table table, string name, TOM.DataType dataType = TOM.DataType.Int64, bool isKey = false)
        {
            var column = new TOM.DataColumn() { Name = name, DataType = dataType, IsKey = isKey };
            table.Columns.Add(column);
            return column;
        }

        public static TOM.DataColumn AddGroupingColumn(this TOM.DataColumn column, TOM.DataColumn groupingColumn)
        {
            column.RelatedColumnDetails ??= new TOM.RelatedColumnDetails();
            column.RelatedColumnDetails.GroupByColumns.Add(new TOM.GroupByColumn { GroupingColumn = groupingColumn });
            return column;
        }
    }
}
