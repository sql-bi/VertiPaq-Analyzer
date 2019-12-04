using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaTable
    {
        private Dax.Metadata.Table Table;

        internal VpaTable( Metadata.Table table )
        {
            this.Table = table;
        }
        
        public string TableName { get { return this.Table.TableName.ToString(); } }
        
        public string TableExpression { get { return this.Table.TableExpression?.ToString(); } }
        public long RowsCount { get { return this.Table.RowsCount; } }
        public long ReferentialIntegrityViolationCount { get { return this.Table.ReferentialIntegrityViolationCount; } }

        public string ColumnsEncoding {
            get {
                string minEncoding = this.Table.Columns.Min(c => c.Encoding) ?? "?";
                string maxEncoding = this.Table.Columns.Max(c => c.Encoding) ?? "?";
                return (minEncoding.Equals(maxEncoding) ? minEncoding : "Many");
            }
        }

        public long ColumnsDictionarySize { get { return this.Table.ColumnsDictionarySize; } }
        
        public long ColumnsDataSize { get { return this.Table.ColumnsDataSize; } }

        public long ColumnsHierarchiesSize { get { return this.Table.ColumnsHierarchiesSize; } }

        public long ColumnsTotalSize { get { return this.Table.ColumnsTotalSize; } }

        public long TableSize { get { return this.Table.TableSize; } }
        public long RelationshipsSize { get { return this.Table.RelationshipsSize; } }
        public long UserHierarchiesSize { get { return this.Table.UserHierarchiesSize; } }

        public double PercentageDatabase {
            get {
                double modelSize = this.Table.Model.Tables.Sum(t => t.TableSize);
                double tableSize = this.TableSize;
                return tableSize / modelSize;
            }
        }

        public int SegmentsNumber {
            get {
                return this.Table.Columns.First<Metadata.Column>().ColumnSegments.Count();
            }
        }
        public int PartitionsNumber {
            get {
                return this.Table.Partitions.Count();
            }
        }

        public int ColumnsNumber {
            get {
                return this.Table.Columns.Count();
            }
        }

        public long MaxFromColumnCardinality {
            get {
                var rFrom = this.Table.GetRelationshipsFrom();
                return (rFrom?.Count() > 0) ? rFrom.Max(r => r.FromColumn.ColumnCardinality) : 0;
            }
        }
        public long MaxToColumnCardinality {
            get {
                var rTo = this.Table.GetRelationshipsFrom();
                return (rTo?.Count() > 0) ? rTo.Max(r => r.ToColumn.ColumnCardinality) : 0;
            }
        }

        public IEnumerable<VpaColumn> Columns { get { return from c in this.Table.Columns select new VpaColumn(c); } }

        public IEnumerable<VpaMeasure> Measures { get { return from m in this.Table.Measures select new VpaMeasure(m); } }
        public IEnumerable<VpaUserHierarchy> UserHierarchies { get { return from uh in this.Table.UserHierarchies select new VpaUserHierarchy(uh); } }
        public IEnumerable<VpaPartition> Partitions { get { return from p in this.Table.Partitions select new VpaPartition(p); } }

        public IEnumerable<VpaTablePermission> TablePermissions { get { return from tp in this.Table.GetTablePermissions() select new VpaTablePermission(tp);  } }

        public IEnumerable<VpaRelationship> RelationshipsTo { get { return from r in this.Table.GetRelationshipsTo() select new VpaRelationship(r); } }
        public IEnumerable<VpaRelationship> RelationshipsFrom { get { return from r in this.Table.GetRelationshipsFrom() select new VpaRelationship(r); } }
    }
}
