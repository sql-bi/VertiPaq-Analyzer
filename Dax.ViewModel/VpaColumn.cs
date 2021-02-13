using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Dax.ViewModel
{
    public class VpaColumn
    {
        private readonly Dax.Metadata.Column Column;
        internal VpaColumn(Dax.Metadata.Column column )
        {
            this.Column = column;
        }
        //internal Column()
        //{
        //    ColumnHierarchies = new List<ColumnHierarchy>();
        //    ColumnSegments = new List<ColumnSegment>();
        //}

        public VpaTable Table { get { return new VpaTable(this.Column.Table); } }
        public string ColumnName { get { return this.Column.ColumnName.ToString(); } }
        public string TableColumnName { get { return this.Column.Table.TableName.ToString() + "-" + this.Column.ColumnName.ToString(); } }
        public long TableRowsCount { get { return this.Column.Table.RowsCount; } }
        public long ColumnCardinality { get { return this.Column.ColumnCardinality; } }
        public string DataType { get { return this.Column.DataType; } }
        public bool IsHidden { get { return this.Column.IsHidden; } }
        public string Encoding { get { return this.Column.Encoding; } }
        public string TypeName { get { return this.Column.DataType; } }
        public string ColumnExpression { get { return this.Column.ColumnExpression.ToString(); } }

        public string EncodingHint { get { return this.Column.EncodingHint; } }
        public bool IsAvailableInMDX { get { return this.Column.IsAvailableInMDX; } }
        public bool IsKey { get { return this.Column.IsKey; } }
        public bool IsNullable { get { return this.Column.IsNullable; } }
        public bool IsUnique { get { return this.Column.IsUnique; } }
        public bool KeepUniqueRows { get { return this.Column.KeepUniqueRows; } }
        public string SortByColumnName { get { return this.Column.SortByColumnName; } }
        public string State { get { return this.Column.State; } }
        public bool IsRowNumber { get { return this.Column.IsRowNumber; } }
        public bool IsReferenced { get { return this.Column.IsReferenced; } }

        public long DictionarySize { get { return this.Column.DictionarySize; } }

        public long DataSize {
            get {
                return this.Column.DataSize;
            }
        }


        public long HierarchiesSize {
            get {
                return this.Column.HierarchiesSize;
            }
        }

        public long TotalSize {
            get {
                return this.Column.TotalSize;
            }
        }
        public double? Selectivity {
            get {
                return this.Column.Selectivity;
            }
        }


        public double PercentageDatabase {
            get {
                double modelSize = this.Column.Table.Model.Tables.Sum(t => t.TableSize);
                double columnSize = this.TotalSize;
                return columnSize / modelSize;
            }
        }
        public double PercentageTable {
            get {
                double tableSize = this.Column.Table.ColumnsTotalSize;
                double columnSize = this.TotalSize;
                return columnSize / tableSize;
            }
        }
        public int SegmentsNumber {
            get {
                return this.Column.ColumnSegments.Count();
            }
        }
        public int PartitionsNumber {
            get {
                return this.Column.Table.Partitions.Count();
            }
        }
        

        // TODO ??
        // public IEnumerable<ColumnHierarchy> ColumnHierarchies { get; }
        // public IEnumerable<ColumnSegment> ColumnSegments { get; }
    }
}
