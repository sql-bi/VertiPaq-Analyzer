using Newtonsoft.Json;
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

        [JsonIgnore]
        public Table Table { get; set; }

        public int Precedence { get; set; }

        public List<CalculationItem> CalculationItems { get; }

        public DaxExpression MultipleOrEmptySelectionExpression { get; set; }

        public DaxExpression MultipleOrEmptySelectionFormatStringExpression { get; set; }

        public DaxExpression NoSelectionExpression { get; set; }

        public DaxExpression NoSelectionFormatStringExpression { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            foreach (var i in CalculationItems)
                i.CalculationGroup = this;            
        }
    }
}
