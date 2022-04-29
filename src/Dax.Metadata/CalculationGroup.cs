using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Dax.Metadata
{
    public class CalculationGroup
    {
        public CalculationGroup(Table table) : this()
        {
            this.Table = table;
        }
        private CalculationGroup()
        {
            this.CalculationItems = new List<CalculationItem>();
        }

        public Table Table { get; set; }

        public int Precedence { get; set; }

        public List<CalculationItem> CalculationItems { get; }
    }
}
