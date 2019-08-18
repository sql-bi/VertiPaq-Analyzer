using System;
using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class ColumnHierarchy
    {
        [JsonIgnore]
        private Dax.Model.ColumnHierarchy _ColumnHierarchy;
        internal ColumnHierarchy(Dax.Model.ColumnHierarchy columnHierarchy)
        {
            _ColumnHierarchy = columnHierarchy;
        }
        private ColumnHierarchy() { }

        public string ColumnName { get { return this._ColumnHierarchy.Column.ColumnName.Name; } }
        public string TableName { get { return this._ColumnHierarchy.Column.Table.TableName.Name; } }
        public string FullColumnName {
            get {
                return Column.GetFullColumnName(this._ColumnHierarchy.Column);
            }
        }

        public string StructureName { get { return this._ColumnHierarchy.StructureName.Name; } }
        public long SegmentNumber { get { return this._ColumnHierarchy.SegmentNumber; } }
        public long TablePartitionNumber { get { return this._ColumnHierarchy.TablePartitionNumber; } }
        public long UsedSize { get { return this._ColumnHierarchy.UsedSize; } }
    }
}
