namespace Dax.Metadata.Tests
{
    using System.Linq;
    using Dax.ViewModel;
    using Xunit;

    public class VpaRelationshipLabelTests
    {
        private static VpaRelationship GetRelationship(string fromCardinalityType, string crossFilteringBehavior, string toCardinalityType)
        {
            var model = new Model();
            var sales = new Table(model) { TableName = new DaxName("Sales") };
            var customer = new Table(model) { TableName = new DaxName("Customer") };
            model.Tables.Add(sales);
            model.Tables.Add(customer);
            var fromColumn = new Column(sales) { ColumnName = new DaxName("CustomerKey") };
            var toColumn = new Column(customer) { ColumnName = new DaxName("CustomerKey") };
            sales.Columns.Add(fromColumn);
            customer.Columns.Add(toColumn);
            model.Relationships.Add(new Relationship(fromColumn, toColumn)
            {
                FromCardinalityType = fromCardinalityType,
                CrossFilteringBehavior = crossFilteringBehavior,
                ToCardinalityType = toCardinalityType
            });
            return new VpaModel(model).Relationships.Single();
        }

        [Theory]
        [InlineData("Many", "OneDirection", "One", "Sales[CustomerKey] ∞←1 Customer[CustomerKey]")]
        [InlineData("Many", "BothDirections", "Many", "Sales[CustomerKey] ∞↔∞ Customer[CustomerKey]")]
        [InlineData("One", "Automatic", "One", "Sales[CustomerKey] 1←1 Customer[CustomerKey]")]
        [InlineData(null, null, null, "Sales[CustomerKey] === Customer[CustomerKey]")]
        public void RelationshipFromToName_SymbolsTest(string fromCardinalityType, string crossFilteringBehavior, string toCardinalityType, string expected)
        {
            var relationship = GetRelationship(fromCardinalityType, crossFilteringBehavior, toCardinalityType);

            Assert.Equal(expected, relationship.RelationshipFromToName);
        }
    }
}
