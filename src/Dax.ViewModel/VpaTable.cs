using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaTable
    {
        private readonly Dax.Metadata.Table Table;

        internal VpaTable( Metadata.Table table )
        {
            this.Table = table;
        }
        
        public string TableName { get { return this.Table.TableName.ToString(); } }
        
        public string TableExpression { get { return this.Table.TableExpression?.Expression; } }
        public long RowsCount { get { return this.Table.RowsCount; } }
        public long ReferentialIntegrityViolationCount { get { return this.Table.ReferentialIntegrityViolationCount; } }

        /// <summary>
        /// HASH or VALUE when all the columns with a known encoding share it, Many when they differ,
        /// ? when no column has an encoding. Columns without an encoding are ignored.
        /// </summary>
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
        public bool IsReferenced { get { return this.Table.IsReferenced; } }
        public bool? IsDateTable { get { return this.Table.IsDateTable; } }
        public bool? HasDirectQueryPartitions { get { return this.Table.HasDirectQueryPartitions; } }
        public bool? HasDualPartitions { get { return this.Table.HasDualPartitions; } }

        public double PercentageDatabase {
            get {
                double modelSize = this.Table.Model.Tables.Sum(t => t.TableSize);
                double tableSize = this.TableSize;
                return modelSize == 0 ? 0 : tableSize / modelSize;
            }
        }

        public int SegmentsNumber {
            get {
                return this.Table.Columns.FirstOrDefault<Metadata.Column>()?.ColumnSegments.Count ?? 0;
            }
        }

        public int SegmentsTotalNumber
        {
            get
            {
                return this.Table.Columns.Sum<Metadata.Column>( c => c.ColumnSegments.Count );
            }
        }

        public int? SegmentsPageable
        {
            get
            {
                var pageable = this.Table.Columns.Select<Metadata.Column, int?>(
                    c => c.ColumnSegments.Any(s => s.IsPageable.HasValue == true) ?
                        c.ColumnSegments.Count(s => s.IsPageable == true) :
                        (int?)null
                ).ToList();
                return pageable.Any(v => v.HasValue) ? pageable.Sum() : null;
            }
        }

        public int? SegmentsResident
        {
            get
            {
                var resident = this.Table.Columns.Select<Metadata.Column, int?>(
                    c => (c.ColumnSegments.Any(s => s.IsResident.HasValue == true)) ?
                        c.ColumnSegments.Count(s => s.IsResident == true) :
                        (int?)null
                ).ToList();
                return resident.Any(v => v.HasValue) ? resident.Sum() : null;
            }
        }

        public double? SegmentsAverageTemperature
        {
            get
            {
                return this.Table.Columns.Average<Metadata.Column>(
                    c => c.ColumnSegments.Average(s => s.Temperature ) 
                );
            }
        }

        public DateTime? SegmentsLastAccessed
        {
            get
            {
                var q = from c in this.Table.Columns select c.ColumnSegments.Max(s => s.LastAccessed);
                return q.Max();
            }
        }


        public int PartitionsNumber {
            get {
                return this.Table.Partitions.Count;
            }
        }

        public int ColumnsNumber {
            get {
                return this.Table.Columns.Count;
            }
        }

        /// <summary>
        /// Largest cardinality of the From columns of the relationships that start from this table; 0 without relationships.
        /// </summary>
        public long MaxFromColumnCardinality {
            get {
                var rFrom = this.Table.GetRelationshipsFrom();
                return (rFrom?.Any() == true) ? rFrom.Max(r => r.FromColumn.ColumnCardinality) : 0;
            }
        }
        /// <summary>
        /// Largest cardinality of the To columns of the relationships that start from this table; 0 without relationships.
        /// The relationships that end at this table are not considered.
        /// </summary>
        public long MaxToColumnCardinality {
            get {
                var rFrom = this.Table.GetRelationshipsFrom();
                return (rFrom?.Any() == true) ? rFrom.Max(r => r.ToColumn.ColumnCardinality) : 0;
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
