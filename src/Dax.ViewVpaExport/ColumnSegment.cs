using System;
using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class ColumnSegment
    {
        [JsonIgnore]
        private readonly Dax.Metadata.ColumnSegment _ColumnSegment;

        internal ColumnSegment(Dax.Metadata.ColumnSegment columnSegment)
        {
            this._ColumnSegment = columnSegment;
        }

        private ColumnSegment() { }

        public string ColumnName => this._ColumnSegment.Column.ColumnName.Name; 
        public string TableName => this._ColumnSegment.Column.Table.TableName.Name; 
        public string FullColumnName {
            get {
                return Column.GetFullColumnName(this._ColumnSegment.Column);
            }
        }
        public string PartitionName => this._ColumnSegment.Partition.PartitionName.Name;
        public string PartitionState => this._ColumnSegment.Partition.State?.ToString();
        public string PartitionType => this._ColumnSegment.Partition.Type?.ToString();
        public string PartitionMode => this._ColumnSegment.Partition.Mode?.ToString();
        public DateTime? RefreshedTime => this._ColumnSegment.Partition.RefreshedTime;


        public long SegmentNumber => this._ColumnSegment.SegmentNumber;
        /// <summary>
        /// The number of the partition of the segment: Dax.Metadata.ColumnSegment has no such field,
        /// and the extractor writes the TABLE_PARTITION_NUMBER of the segments on the partition.
        /// ColumnHierarchy.TablePartitionNumber is instead a field of the hierarchy.
        /// </summary>
        public long TablePartitionNumber => this._ColumnSegment.Partition.PartitionNumber;
        public long SegmentRows => this._ColumnSegment.SegmentRows;
        public long UsedSize => this._ColumnSegment.UsedSize;
        public string CompressionType => this._ColumnSegment.CompressionType;
        public long BitsCount => this._ColumnSegment.BitsCount;
        public long BookmarkBitsCount => this._ColumnSegment.BookmarkBitsCount;
        public string VertipaqState => this._ColumnSegment.VertipaqState;

        public bool? IsPageable => this._ColumnSegment.IsPageable;
        public bool? IsResident => this._ColumnSegment.IsResident;
        public double? Temperature => this._ColumnSegment.Temperature;
        public DateTime? LastAccessed => this._ColumnSegment.LastAccessed;

    }
}
