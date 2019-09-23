using System;
using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class ColumnSegment
    {
        [JsonIgnore]
        private Dax.Metadata.ColumnSegment _ColumnSegment;

        internal ColumnSegment(Dax.Metadata.ColumnSegment columnSegment)
        {
            this._ColumnSegment = columnSegment;
        }

        private ColumnSegment() { }

        public string ColumnName { get { return this._ColumnSegment.Column.ColumnName.Name; } }
        public string TableName { get { return this._ColumnSegment.Column.Table.TableName.Name; } }
        public string FullColumnName {
            get {
                return Column.GetFullColumnName(this._ColumnSegment.Column);
            }
        }
        public string PartitionName { get { return this._ColumnSegment.Partition.PartitionName.ToString(); } }

        public long SegmentNumber { get { return this._ColumnSegment.SegmentNumber; } }
        public long TablePartitionNumber { get { return this._ColumnSegment.Partition.PartitionNumber; } }
        public long SegmentRows { get { return this._ColumnSegment.SegmentRows; } }
        public long UsedSize { get { return this._ColumnSegment.UsedSize; } }
        public string CompressionType { get { return this._ColumnSegment.CompressionType; } }
        public long BitsCount { get { return this._ColumnSegment.BitsCount; } }
        public long BookmarkBitsCount { get { return this._ColumnSegment.BookmarkBitsCount; } }
        public string VertipaqState { get { return this._ColumnSegment.VertipaqState; }
}
    }
}
