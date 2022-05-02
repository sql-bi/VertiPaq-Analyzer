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
        public string PartitionName => this._ColumnSegment.Partition.PartitionName.ToString();
        public string PartitionState => this._ColumnSegment.Partition.State?.ToString();
        public string PartitionType => this._ColumnSegment.Partition.Type?.ToString();
        public DateTime? RefreshedTime => this._ColumnSegment.Partition.RefreshedTime;


        public long SegmentNumber => this._ColumnSegment.SegmentNumber;
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
