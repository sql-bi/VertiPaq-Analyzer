using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Model
{
    public class ColumnSegment
    {
        public ColumnSegment ( Column column, Partition partition )
        {
            Column = column;
            Partition = partition;
        }

        private ColumnSegment() { }

        public Column Column { get; set; }
        public Partition Partition { get; set; }
        public long SegmentNumber { get; set; }
        public long SegmentRows { get; set; }
        public long UsedSize { get; set; }
        public string CompressionType { get; set; }
        public long BitsCount { get; set; }
        public long BookmarkBitsCount { get; set; }
        public string VertipaqState { get; set; }
    }
}
