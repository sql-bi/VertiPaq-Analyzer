using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Model
{
    public class Measure
    {
        public Table Table { get; set; }
        public DaxName MeasureName { get; set; }
        public DaxExpression MeasureExpression { get; set; }
        public string DisplayFolder { get; set; }
        public string Description { get; set; }

        public bool IsHidden { get; set; }
        public string DataType { get; set; }
        public DaxExpression DetailRowsExpression { get; set; }
        public string FormatString { get; set; }
        public DaxExpression KpiStatusExpression { get; set; }
        public DaxExpression KpiTargetExpression { get; set; }
        public string KpiTargetFormatString { get; set; }
        public DaxExpression KpiTrendExpression { get; set; }
        
    }
}
