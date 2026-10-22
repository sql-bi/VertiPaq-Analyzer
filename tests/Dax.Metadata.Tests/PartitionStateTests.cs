namespace Dax.Metadata.Tests
{
    using System.Linq;
    using Dax.ViewModel;
    using Xunit;

    public class PartitionStateTests
    {
        private static Model GetModel(Partition.PartitionState? state)
        {
            var model = new Model();
            var table = new Table(model) { TableName = new DaxName("Sales") };
            model.Tables.Add(table);
            var column = new Column(table) { ColumnName = new DaxName("Amount") };
            table.Columns.Add(column);
            var partition = new Partition(table) { PartitionName = new DaxName("Sales"), State = state };
            table.Partitions.Add(partition);
            column.ColumnSegments.Add(new ColumnSegment(column, partition));
            return model;
        }

        [Fact]
        public void PartitionState_StoredValueOneIsReadyTest()
        {
            // DaxModel.json stores the number of TOM ObjectState, where 1 is Ready
            Assert.Equal("Ready", ((Partition.PartitionState)1).ToString());
        }

        [Fact]
        public void PartitionState_ExportReturnsReadyTest()
        {
            var segment = new ViewVpaExport.Model(GetModel((Partition.PartitionState)1)).ColumnsSegments.Single();

            Assert.Equal("Ready", segment.PartitionState);
        }

        [Fact]
        public void PartitionState_ViewModelReturnsReadyTest()
        {
            var partition = new VpaModel(GetModel((Partition.PartitionState)1)).Tables.Single().Partitions.Single();

            Assert.Equal("Ready", partition.PartitionState);
        }
    }
}
