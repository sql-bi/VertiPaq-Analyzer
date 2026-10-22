namespace Dax.Metadata.Tests
{
    using System.Linq;
    using Dax.ViewModel;
    using Xunit;

    public class VpaPartitionDescriptionTests
    {
        private static VpaPartition GetPartition(DaxNote description)
        {
            var model = new Model();
            var table = new Table(model) { TableName = new DaxName("Sales") };
            model.Tables.Add(table);
            table.Columns.Add(new Column(table) { ColumnName = new DaxName("Amount") });
            table.Partitions.Add(new Partition(table) { PartitionName = new DaxName("Sales"), Description = description });

            return new VpaModel(model).Tables.Single().Partitions.Single();
        }

        [Fact]
        public void Description_NullWhenDescriptionIsMissingTest()
        {
            // Partitions of files written before Dax.Metadata 1.2.0 have no Description
            var partition = GetPartition(null);

            Assert.Null(partition.Description);
        }

        [Fact]
        public void Description_NullWhenNoteIsNullTest()
        {
            var partition = GetPartition(new DaxNote(null));

            Assert.Null(partition.Description);
        }

        [Fact]
        public void Description_ReturnsNoteTest()
        {
            var partition = GetPartition(new DaxNote("Current year"));

            Assert.Equal("Current year", partition.Description);
        }
    }
}
