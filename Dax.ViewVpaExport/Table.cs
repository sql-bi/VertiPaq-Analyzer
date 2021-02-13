using System;
using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class Table
    {
        [JsonIgnore]
        private readonly Dax.Metadata.Table _Table;

        internal Table(Dax.Metadata.Table table)
        {
            this._Table = table;
        }

        public string TableName { get { return this._Table.TableName.Name; } }

        public string TableExpression { get { return this._Table.TableExpression?.Expression; } }
        public long RowsCount { get { return this._Table.RowsCount; } }
        public long ReferentialIntegrityViolationCount { get { return this._Table.ReferentialIntegrityViolationCount; } }
        public bool IsHidden { get { return this._Table.IsHidden; } }
        public string Description { get { return this._Table.Description; } }

        public long ColumnsSize { get { return this._Table.ColumnsTotalSize; } }

        public long TableSize { get { return this._Table.TableSize; } }
        public long RelationshipsSize { get { return this._Table.RelationshipsSize; } }
        public long UserHierarchiesSize { get { return this._Table.UserHierarchiesSize; } }
        public bool IsReferenced { get { return this._Table.IsReferenced; } }
    }
}
