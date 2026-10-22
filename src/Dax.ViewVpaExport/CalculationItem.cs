using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class CalculationItem
        {
        [JsonIgnore]
        private readonly Dax.Metadata.CalculationItem _CalculationItem;
        internal CalculationItem(Dax.Metadata.CalculationItem calculationItem)
        {
            this._CalculationItem = calculationItem;
        }

        public string CalculationGroup { get { return this._CalculationItem.CalculationGroup.Table.TableName.Name; } }
        public string Attribute { get { return this._CalculationItem.CalculationGroup.Table.Columns.Find(c => c.IsCalculationGroupAttribute == true)?.ColumnName.Name; } }
        public bool IsHidden {
            get {
                var column = this._CalculationItem.CalculationGroup.Table.Columns.Find(c => c.IsCalculationGroupAttribute == true);
                return (column != null) && column.IsHidden;
            }
        }
        /// <summary>
        /// Precedence of the calculation group of the item: a 32-bit integer, as in TOM, while the counts and sizes of the export are 64-bit.
        /// </summary>
        public int Precedence { get { return this._CalculationItem.CalculationGroup.Precedence; } }
        public string ItemName { get { return this._CalculationItem.ItemName.Name; } }
        public string ItemExpression { get { return this._CalculationItem.ItemExpression?.Expression; } }
        public string State { get { return this._CalculationItem.State; } }
        public string ErrorMessage { get { return this._CalculationItem.ErrorMessage; } }
        public string FormatStringDefinition { get { return this._CalculationItem.FormatStringDefinition?.Expression; } }
        public string FormatStringState { get { return this._CalculationItem.FormatStringState; } }
        public string FormatStringErrorMessage { get { return this._CalculationItem.FormatStringErrorMessage; } }
        public bool IsReferenced { get { return this._CalculationItem.IsReferenced; } }
        public string Description { get { return this._CalculationItem.Description?.Note; } }

    }
}
