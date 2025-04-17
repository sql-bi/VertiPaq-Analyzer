using Newtonsoft.Json;
using System.Diagnostics;

namespace Dax.Metadata
{
    [DebuggerDisplay("{MeasureName.Name,nq}")]
    public class Measure
    {
        [JsonIgnore]
        public Table Table { get; set; }
        public DaxName MeasureName { get; set; }
        public DaxExpression MeasureExpression { get; set; }
        public DaxExpression FormatStringExpression { get; set; }

        public DaxNote DisplayFolder { get; set; } 
        public DaxNote Description { get; set; }

        public bool IsHidden { get; set; }
        public string DataType { get; set; }
        public DaxExpression DetailRowsExpression { get; set; }
        public string FormatString { get; set; }
        public DaxExpression KpiStatusExpression { get; set; }
        public DaxExpression KpiTargetExpression { get; set; }
        public string KpiTargetFormatString { get; set; }
        public DaxExpression KpiTrendExpression { get; set; }

        public bool IsReferenced { get; set; }

    }
}
