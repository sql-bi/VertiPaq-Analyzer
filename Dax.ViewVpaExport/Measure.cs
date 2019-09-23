using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class Measure
    {
        [JsonIgnore]
        private Dax.Metadata.Measure _Measure;
        internal Measure(Dax.Metadata.Measure measure)
        {
            this._Measure = measure;
        }

        public string MeasureName { get { return this._Measure.MeasureName.Name; } }
        public string TableName { get { return this._Measure.Table.TableName.Name; } }
        public string FullMeasureName {
            get {
                return string.Format("'{0}'[{1}]", TableName, MeasureName);
            }
        }

        public string MeasureExpression { get { return this._Measure.MeasureExpression?.Expression; } }
        public string DisplayFolder { get { return this._Measure.DisplayFolder; } }
        public string Description { get { return this._Measure.Description; } }
        public bool IsHidden { get { return this._Measure.IsHidden; } }
        public string DataType { get { return this._Measure.DataType; } }
        public string DetailRowsExpression { get { return this._Measure.DetailRowsExpression?.Expression; } }
        public string FormatString { get { return this._Measure.FormatString; } }
        public string KpiStatusExpression { get { return this._Measure.KpiStatusExpression?.Expression; } }
        public string KpiTargetExpression { get { return this._Measure.KpiTargetExpression?.Expression; } }
        public string KpiTargetFormatString { get { return this._Measure.KpiTargetFormatString; } }
        public string KpiTrendExpression { get { return this._Measure.KpiTrendExpression?.Expression; } }

    }
}
