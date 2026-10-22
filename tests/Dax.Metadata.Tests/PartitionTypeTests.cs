namespace Dax.Metadata.Tests
{
    using System.Linq;
    using Xunit;

    public class PartitionTypeTests
    {
        private static ViewVpaExport.ColumnSegment GetSegment(Partition.PartitionType type)
        {
            var model = new Model();
            var table = new Table(model) { TableName = new DaxName("Sales") };
            model.Tables.Add(table);
            var column = new Column(table) { ColumnName = new DaxName("Amount") };
            table.Columns.Add(column);
            var partition = new Partition(table) { PartitionName = new DaxName("Sales"), Type = type };
            table.Partitions.Add(partition);
            column.ColumnSegments.Add(new ColumnSegment(column, partition));

            return new ViewVpaExport.Model(model).ColumnsSegments.Single();
        }

        [Fact]
        public void PartitionType_ParquetTest()
        {
            // TOM PartitionSourceType.Parquet is 9
            var segment = GetSegment((Partition.PartitionType)9);

            Assert.Equal("Parquet", segment.PartitionType);
        }

        [Fact]
        public void PartitionType_UnknownValuePrintsAsNumberTest()
        {
            var segment = GetSegment((Partition.PartitionType)99);

            Assert.Equal("99", segment.PartitionType);
        }
    }
}
