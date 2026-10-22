namespace Dax.Metadata.Tests
{
    using System.Linq;
    using Dax.ViewModel;
    using Xunit;

    public class VpaTableTableExpressionTests
    {
        private static VpaTable GetTable(DaxExpression tableExpression)
        {
            var model = new Model();
            model.Tables.Add(new Table(model) { TableName = new DaxName("Dates"), TableExpression = tableExpression });

            return new VpaModel(model).Tables.Single();
        }

        [Fact]
        public void TableExpression_ReturnsExpressionTest()
        {
            var table = GetTable(new DaxExpression("CALENDARAUTO()"));

            Assert.Equal("CALENDARAUTO()", table.TableExpression);
        }

        [Fact]
        public void TableExpression_NullWhenExpressionIsNullTest()
        {
            // A file with "TableExpression": null is read as a DaxExpression with a null Expression
            var table = GetTable(new DaxExpression(null));

            Assert.Null(table.TableExpression);
        }

        [Fact]
        public void TableExpression_NullWhenTableExpressionIsMissingTest()
        {
            var table = GetTable(null);

            Assert.Null(table.TableExpression);
        }
    }
}
