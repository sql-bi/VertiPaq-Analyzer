namespace Dax.Metadata.Tests
{
    using System.Linq;
    using Dax.ViewModel;
    using Xunit;

    public class FullNameEscapingTests
    {
        private static Model GetModel(string tableName, string columnName, string measureName)
        {
            var model = new Model();
            var table = new Table(model) { TableName = new DaxName(tableName) };
            model.Tables.Add(table);
            var column = new Column(table) { ColumnName = new DaxName(columnName) };
            table.Columns.Add(column);
            table.Measures.Add(new Measure { Table = table, MeasureName = new DaxName(measureName) });
            var partition = new Partition(table) { PartitionName = new DaxName(tableName) };
            table.Partitions.Add(partition);
            column.ColumnSegments.Add(new ColumnSegment(column, partition));
            column.ColumnHierarchies.Add(new ColumnHierarchy(column) { StructureName = new DaxName("H$") });
            model.Relationships.Add(new Relationship(column, column));
            return model;
        }

        [Fact]
        public void FullNames_EscapeQuoteAndBracketTest()
        {
            var model = GetModel("Bob's", "Price]", "Total [Sales]");
            var export = new ViewVpaExport.Model(model);
            var relationship = new VpaModel(model).Relationships.Single();

            Assert.Equal("'Bob''s'[Price]]]", export.Columns.Single().FullColumnName);
            Assert.Equal("'Bob''s'[Price]]]", export.ColumnsSegments.Single().FullColumnName);
            Assert.Equal("'Bob''s'[Price]]]", export.ColumnsHierarchies.Single().FullColumnName);
            Assert.Equal("'Bob''s'[Price]]]", export.Relationships.Single().FromFullColumnName);
            Assert.Equal("'Bob''s'[Price]]]", export.Relationships.Single().ToFullColumnName);
            Assert.Equal("'Bob''s'[Total [Sales]]]", export.Measures.Single().FullMeasureName);
            Assert.Equal("'Bob''s'[Price]]]", relationship.FromColumnName);
            Assert.Equal("'Bob''s'[Price]]]", relationship.ToColumnName);
        }

        [Fact]
        public void FullNames_UnchangedWithoutSpecialCharactersTest()
        {
            var model = GetModel("Sales", "Amount", "Total Sales");
            var export = new ViewVpaExport.Model(model);

            Assert.Equal("'Sales'[Amount]", export.Columns.Single().FullColumnName);
            Assert.Equal("'Sales'[Total Sales]", export.Measures.Single().FullMeasureName);
            Assert.Equal("'Sales'[Amount]", new VpaModel(model).Relationships.Single().FromColumnName);
        }
    }
}
