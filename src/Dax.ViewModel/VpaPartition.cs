using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaPartition
    {
        private Dax.Metadata.Partition Partition;

        internal VpaPartition(Dax.Metadata.Partition partition)
        {
            this.Partition = partition;
        }

        public string PartitionName => this.Partition.PartitionName.ToString();

        public long RowsCount => this.Partition.Table.Columns.Max(c => c.ColumnSegments.Where(cs => cs.Partition.PartitionName.ToString() == this.PartitionName).Sum(cs => cs.SegmentRows));
        public long DataSize => this.Partition.Table.Columns.Sum(c => c.ColumnSegments.Where(cs => cs.Partition.PartitionName.ToString() == this.PartitionName).Sum(cs => cs.UsedSize));
        public long SegmentsNumber => this.Partition.Table.Columns.Max(c => c.ColumnSegments.Where(cs => cs.Partition.PartitionName.ToString() == this.PartitionName).Count());

        public string Description => this.Partition.Description.ToString();
        public string PartitionState => this.Partition.State.ToString();
        public string PartitionType => this.Partition.Type.ToString();
        public string PartitionMode => this.Partition.Mode.ToString();
        public DateTime? RefreshedTime => this.Partition.RefreshedTime;

        public int SegmentsTotalNumber
        {
            get
            {
                return this.Partition.Table.Columns.Sum<Metadata.Column>(c => c.ColumnSegments.Where(cs => cs.Partition.PartitionName.ToString() == this.PartitionName).Count());
            }
        }

        public int? SegmentsPageable
        {
            get
            {
                return this.Partition.Table.Columns.Sum<Metadata.Column>(
                    c => (c.ColumnSegments.Where(cs => cs.Partition.PartitionName.ToString() == this.PartitionName).Any(s => s.IsPageable.HasValue == true)) ?
                        c.ColumnSegments.Where(cs => cs.Partition.PartitionName.ToString() == this.PartitionName).Count(s => s.IsPageable == true) :
                        (int?)null
                );
            }
        }

        public int? SegmentsResident
        {
            get
            {
                return this.Partition.Table.Columns.Sum<Metadata.Column>(
                    c => (c.ColumnSegments.Where(cs => cs.Partition.PartitionName.ToString() == this.PartitionName).Any(s => s.IsResident.HasValue == true)) ?
                        c.ColumnSegments.Where(cs => cs.Partition.PartitionName.ToString() == this.PartitionName).Count(s => s.IsResident == true) :
                        (int?)null
                );
            }
        }

        public double? SegmentsAverageTemperature
        {
            get
            {
                return this.Partition.Table.Columns.Average<Metadata.Column>(
                    c => c.ColumnSegments.Where(cs => cs.Partition.PartitionName.ToString() == this.PartitionName).Average(s => s.Temperature)
                );
            }
        }

        public DateTime? SegmentsLastAccessed
        {
            get
            {
                var q = from c in this.Partition.Table.Columns select c.ColumnSegments.Where(cs => cs.Partition.PartitionName.ToString() == this.PartitionName).Max(s => s.LastAccessed);
                return q.Max();
            }
        }

    }
}
