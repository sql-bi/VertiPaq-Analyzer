namespace Dax.Metadata.Tests
{
    using System.Linq;
    using Dax.ViewModel;
    using Xunit;

    public class VpaPartitionEnumTests
    {
        private static VpaPartition GetPartition(Partition.PartitionType? type, Partition.PartitionMode? mode)
        {
            var model = new Model();
            var table = new Table(model) { TableName = new DaxName("Sales") };
            model.Tables.Add(table);
            table.Columns.Add(new Column(table) { ColumnName = new DaxName("Amount") });
            table.Partitions.Add(new Partition(table) { PartitionName = new DaxName("Sales"), Type = type, Mode = mode });

            return new VpaModel(model).Tables.Single().Partitions.Single();
        }

        [Fact]
        public void PartitionEnums_NullWhenMissingTest()
        {
            // Partitions of files written before Dax.Metadata 1.2.0 have no State, Type and Mode
            var partition = GetPartition(null, null);

            Assert.Null(partition.PartitionState);
            Assert.Null(partition.PartitionType);
            Assert.Null(partition.PartitionMode);
        }

        [Fact]
        public void PartitionEnums_ReturnNamesTest()
        {
            var partition = GetPartition(Partition.PartitionType.M, Partition.PartitionMode.Import);

            Assert.Equal("M", partition.PartitionType);
            Assert.Equal("Import", partition.PartitionMode);
        }
    }
}
