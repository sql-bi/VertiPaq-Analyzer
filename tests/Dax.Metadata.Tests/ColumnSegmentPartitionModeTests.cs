namespace Dax.Metadata.Tests
{
    using System.Linq;
    using Xunit;

    public class ColumnSegmentPartitionModeTests
    {
        private static ViewVpaExport.ColumnSegment GetSegment(Partition.PartitionType? type, Partition.PartitionMode? mode)
        {
            var model = new Model();
            var table = new Table(model) { TableName = new DaxName("Sales") };
            model.Tables.Add(table);
            var column = new Column(table) { ColumnName = new DaxName("Amount") };
            table.Columns.Add(column);
            var partition = new Partition(table) { PartitionName = new DaxName("Sales"), Type = type, Mode = mode };
            table.Partitions.Add(partition);
            column.ColumnSegments.Add(new ColumnSegment(column, partition));

            return new ViewVpaExport.Model(model).ColumnsSegments.Single();
        }

        [Theory]
        [InlineData(Partition.PartitionType.M, Partition.PartitionMode.Import, "Import")]
        [InlineData(Partition.PartitionType.M, Partition.PartitionMode.DirectQuery, "DirectQuery")]
        [InlineData(Partition.PartitionType.Calculated, Partition.PartitionMode.Dual, "Dual")]
        [InlineData(Partition.PartitionType.Entity, Partition.PartitionMode.DirectLake, "DirectLake")]
        public void PartitionMode_ReturnsPartitionModeTest(Partition.PartitionType type, Partition.PartitionMode mode, string expected)
        {
            var segment = GetSegment(type, mode);

            Assert.Equal(type.ToString(), segment.PartitionType);
            Assert.Equal(expected, segment.PartitionMode);
        }

        [Fact]
        public void PartitionMode_NullWhenModeIsMissingTest()
        {
            var segment = GetSegment(Partition.PartitionType.M, null);

            Assert.Equal("M", segment.PartitionType);
            Assert.Null(segment.PartitionMode);
        }
    }
}
