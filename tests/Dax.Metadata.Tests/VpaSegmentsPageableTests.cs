namespace Dax.Metadata.Tests
{
    using System.Linq;
    using Dax.ViewModel;
    using Xunit;

    public class VpaSegmentsPageableTests
    {
        private static VpaTable GetTable(bool? isPageable, bool? isResident)
        {
            var model = new Model();
            var table = new Table(model) { TableName = new DaxName("Sales") };
            model.Tables.Add(table);
            var partition = new Partition(table) { PartitionName = new DaxName("Sales") };
            table.Partitions.Add(partition);
            for (int c = 0; c < 2; c++)
            {
                var column = new Column(table) { ColumnName = new DaxName("Column" + c) };
                table.Columns.Add(column);
                column.ColumnSegments.Add(new ColumnSegment(column, partition) { IsPageable = isPageable, IsResident = isResident });
            }
            return new VpaModel(model).Tables.Single();
        }

        [Fact]
        public void SegmentsPageable_NullWhenNoSegmentHasTheStatisticTest()
        {
            // Import models have null IsPageable and IsResident on every segment
            var table = GetTable(null, null);
            var partition = table.Partitions.Single();

            Assert.Null(table.SegmentsPageable);
            Assert.Null(table.SegmentsResident);
            Assert.Null(partition.SegmentsPageable);
            Assert.Null(partition.SegmentsResident);
            Assert.All(table.Columns, c => Assert.Null(c.SegmentsPageable));
        }

        [Fact]
        public void SegmentsPageable_CountsSegmentsTest()
        {
            var table = GetTable(true, false);
            var partition = table.Partitions.Single();

            Assert.Equal(2, table.SegmentsPageable);
            Assert.Equal(0, table.SegmentsResident);
            Assert.Equal(2, partition.SegmentsPageable);
            Assert.Equal(0, partition.SegmentsResident);
        }
    }
}
