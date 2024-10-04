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

        public string TableName => this._Table.TableName.Name;

        public string TableExpression => this._Table.TableExpression?.Expression;
        public long RowsCount => this._Table.RowsCount;
        public long ReferentialIntegrityViolationCount => this._Table.ReferentialIntegrityViolationCount;
        public bool IsHidden => this._Table.IsHidden;
        public bool IsPrivate => this._Table.IsPrivate;
        public bool IsLocalDateTable => this._Table.IsLocalDateTable;
        public bool IsTemplateDateTable => this._Table.IsTemplateDateTable;

        public string Description { get { return this._Table.Description?.Note; } }

        public long ColumnsSize { get { return this._Table.ColumnsTotalSize; } }

        public long TableSize { get { return this._Table.TableSize; } }
        public long RelationshipsSize { get { return this._Table.RelationshipsSize; } }
        public long UserHierarchiesSize { get { return this._Table.UserHierarchiesSize; } }
        public bool IsReferenced { get { return this._Table.IsReferenced; } }
        public string DefaultDetailRowsExpression { get { return this._Table.DefaultDetailRowsExpression?.Expression; } }
    }
}
