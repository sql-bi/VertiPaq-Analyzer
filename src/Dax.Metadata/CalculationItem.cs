using System.Diagnostics;

namespace Dax.Metadata
{
    [DebuggerDisplay("{ItemName.Name,nq}")]
    public class CalculationItem
    {
        public CalculationItem(CalculationGroup calculationGroup) : this()
        {
            this.CalculationGroup = calculationGroup;
        }
        private CalculationItem() { }

        public CalculationGroup CalculationGroup { get; set; }

        public DaxName ItemName { get; set; }

        public DaxExpression ItemExpression { get; set; }
        public string State { get; set; }
        public string ErrorMessage { get; set; }
        public DaxExpression FormatStringDefinition { get; set; }
        public string FormatStringState { get; set; }
        public string FormatStringErrorMessage { get; set; }
        public DaxNote Description { get; set; }

        public bool IsReferenced { get; set; }

    }
}
