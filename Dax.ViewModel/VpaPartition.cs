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
    }
}
