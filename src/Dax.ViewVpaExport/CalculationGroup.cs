namespace Dax.ViewVpaExport
{
    public class CalculationGroup
    {
        private readonly Dax.Metadata.CalculationGroup _calculationGroup;

        internal CalculationGroup(Dax.Metadata.CalculationGroup calculationGroup)
        {
            _calculationGroup = calculationGroup;
        }

        public string MultipleOrEmptySelectionExpression => _calculationGroup.MultipleOrEmptySelectionExpression?.Expression;
        public string MultipleOrEmptySelectionFormatStringExpression => _calculationGroup.MultipleOrEmptySelectionFormatStringExpression?.Expression;
        public string NoSelectionExpression => _calculationGroup.NoSelectionExpression?.Expression;
        public string NoSelectionFormatStringExpression => _calculationGroup.NoSelectionFormatStringExpression?.Expression;
        public int Precedence => _calculationGroup.Precedence;
        public string TableName => _calculationGroup.Table.TableName.Name;
    }
}
