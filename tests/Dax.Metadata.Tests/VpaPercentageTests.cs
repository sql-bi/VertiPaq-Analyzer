namespace Dax.Metadata.Tests
{
    using System.Linq;
    using Dax.ViewModel;
    using Xunit;

    public class VpaPercentageTests
    {
        private static VpaModel GetModel(long dictionarySize)
        {
            var model = new Model();
            var table = new Table(model) { TableName = new DaxName("Sales") };
            model.Tables.Add(table);
            table.Columns.Add(new Column(table) { ColumnName = new DaxName("Amount"), DictionarySize = dictionarySize });
            table.Columns.Add(new Column(table) { ColumnName = new DaxName("Quantity"), DictionarySize = dictionarySize });
            return new VpaModel(model);
        }

        [Fact]
        public void Percentages_ZeroWhenSizeIsZeroTest()
        {
            // A model without data in memory has size 0
            var model = GetModel(0);

            Assert.Equal(0, model.Tables.Single().PercentageDatabase);
            Assert.All(model.Columns, c => Assert.Equal(0, c.PercentageDatabase));
            Assert.All(model.Columns, c => Assert.Equal(0, c.PercentageTable));
        }

        [Fact]
        public void Percentages_ShareOfSizeTest()
        {
            var model = GetModel(100);

            Assert.Equal(1, model.Tables.Single().PercentageDatabase);
            Assert.All(model.Columns, c => Assert.Equal(0.5, c.PercentageDatabase));
            Assert.All(model.Columns, c => Assert.Equal(0.5, c.PercentageTable));
        }
    }
}
