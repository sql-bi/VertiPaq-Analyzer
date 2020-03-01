using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class Column
    {
        #region Utility functions
        internal static string GetFullColumnName(string tableName, string columnName)
        {
            return string.Format("'{0}'[{1}]", tableName, columnName);
        }
        internal static string GetFullColumnName(Dax.Metadata.Column column)
        {
            return GetFullColumnName(column.Table.TableName.Name, column.ColumnName.Name);
        }
        internal static string GetFullColumnName(Column column)
        {
            return GetFullColumnName(column.TableName, column.ColumnName);
        }

        #endregion
        [JsonIgnore]
        private readonly Dax.Metadata.Column _Column;
        internal Column(Dax.Metadata.Column column)
        {
            this._Column = column;
        }
        
        public string ColumnName { get { return this._Column.ColumnName.Name; } }
        public string TableName { get { return this._Column.Table.TableName.Name; } }
        public string FullColumnName {
            get {
                return string.Format("'{0}'[{1}]", TableName, ColumnName);
            }
        }
        public long ColumnCardinality { get { return this._Column.ColumnCardinality; } }
        public string DataType { get { return this._Column.DataType; } }
        public string ColumnType { get { return this._Column.ColumnType; } }
        public bool IsHidden { get { return this._Column.IsHidden; } }
        public string Encoding { get { return this._Column.Encoding; } }
        public string ColumnExpression { get { return this._Column.ColumnExpression?.Expression.ToString(); } }
        public string DisplayFolder { get { return this._Column.DisplayFolder?.Note; } }
        public string Description { get { return this._Column.Description?.Note; } }
        public string FormatString { get { return this._Column.FormatString;  } }

        public string EncodingHint { get { return this._Column.EncodingHint; } }
        public bool IsAvailableInMDX { get { return this._Column.IsAvailableInMDX; } }
        public bool IsKey { get { return this._Column.IsKey; } }
        public bool IsNullable { get { return this._Column.IsNullable; } }
        public bool IsUnique { get { return this._Column.IsUnique; } }
        public bool KeepUniqueRows { get { return this._Column.KeepUniqueRows; } }
        public string SortByColumnName { get { return this._Column.SortByColumnName; } }
        public string State { get { return this._Column.State; } }
        public bool IsRowNumber { get { return this._Column.IsRowNumber; } }

        public long DictionarySize { get { return this._Column.DictionarySize; } }

        public long DataSize {
            get {
                return this._Column.DataSize;
            }
        }


        public long HierarchiesSize {
            get {
                return this._Column.HierarchiesSize;
            }
        }

        public long TotalSize {
            get {
                return this._Column.TotalSize;
            }
        }
        public double? Selectivity {
            get {
                return this._Column.Selectivity;
            }
        }

    }
}
