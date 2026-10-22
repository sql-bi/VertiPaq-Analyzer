namespace Dax.Metadata.Tests
{
    using System.Linq;
    using Dax.ViewModel;
    using Xunit;

    public class VpaTableSegmentsNumberTests
    {
        private static Model GetModel(int columns, int segments)
        {
            var model = new Model();
            var table = new Table(model) { TableName = new DaxName("Sales") };
            model.Tables.Add(table);
            var partition = new Partition(table) { PartitionName = new DaxName("Sales") };
            table.Partitions.Add(partition);
            for (int c = 0; c < columns; c++)
            {
                var column = new Column(table) { ColumnName = new DaxName("Column" + c) };
                table.Columns.Add(column);
                for (int s = 0; s < segments; s++)
                {
                    column.ColumnSegments.Add(new ColumnSegment(column, partition) { SegmentNumber = s, SegmentRows = 100 });
                }
            }
            return model;
        }

        [Fact]
        public void SegmentsNumber_TableWithoutColumnsTest()
        {
            var table = new VpaModel(GetModel(columns: 0, segments: 0)).Tables.Single();

            Assert.Equal(0, table.SegmentsNumber);
            Assert.Equal(0, table.SegmentsTotalNumber);
        }

        [Fact]
        public void SegmentsNumber_PartitionOfTableWithoutColumnsTest()
        {
            var partition = new VpaModel(GetModel(columns: 0, segments: 0)).Tables.Single().Partitions.Single();

            Assert.Equal(0, partition.SegmentsNumber);
            Assert.Equal(0, partition.RowsCount);
            Assert.Equal(0, partition.SegmentsTotalNumber);
        }

        [Fact]
        public void SegmentsNumber_CountOfFirstColumnTest()
        {
            var model = new VpaModel(GetModel(columns: 3, segments: 2));
            var table = model.Tables.Single();
            var partition = table.Partitions.Single();

            Assert.Equal(2, table.SegmentsNumber);
            Assert.Equal(6, table.SegmentsTotalNumber);
            Assert.Equal(2, partition.SegmentsNumber);
            Assert.Equal(200, partition.RowsCount);
        }
    }
}
