using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Dax.Metadata
{
    public class Column
    {
        public Column(Table table) : this()
        {
            this.Table = table;
        }
        private Column()
        {
            ColumnHierarchies = new List<ColumnHierarchy>();
            ColumnSegments = new List<ColumnSegment>();
            GroupByColumns = new List<DaxName>();
        }

        public Table Table { get; set; }
        public DaxName ColumnName { get; set; }
        public long ColumnCardinality { get; set; }
        public string DataType { get; set; }
        public string ColumnType { get; set; }
        public bool IsHidden { get; set; }
        public string Encoding { get; set; }
        public DaxExpression ColumnExpression { get; set; }
        public DaxNote DisplayFolder { get; set; }
        public string FormatString { get; set; }
        public DaxNote Description { get; set; }
        
        public string EncodingHint { get; set; }
        public bool IsAvailableInMDX { get; set; }
        public bool IsKey { get; set; }
        public bool IsNullable { get; set; }
        public bool IsUnique { get; set; }
        public bool KeepUniqueRows { get; set; }
        public DaxName SortByColumnName { get; set; }
        public List<DaxName> GroupByColumns { get; set; }
        public string State { get; set; }
        public bool IsRowNumber { get; set; }
        public bool IsCalculationGroupAttribute { get; set; }
        public bool IsReferenced { get; set; }

        public long DictionarySize { get; set; }

        // TODO - move to DaxName or similar class if we want to serialize this property
        [JsonIgnore]
        public string Dmv1100ColumnId { get; private set; }

        [JsonIgnore]
        public int Dmv1200ColumnId { get; private set; }

        public void SetDmv1100ColumnId( string dmv1100Id )
        {
            Dmv1100ColumnId = dmv1100Id;
            var openIdBracket = dmv1100Id.LastIndexOf('(');
            var closeIdBracket = dmv1100Id.LastIndexOf(')');
            if (openIdBracket >= 0 && closeIdBracket >= 0) {
                var stringDmv1200Id = dmv1100Id.Substring(openIdBracket + 1, closeIdBracket - (openIdBracket + 1));
                if (int.TryParse(stringDmv1200Id, out int intDmv1200Id)) {
                    Dmv1200ColumnId = intDmv1200Id;
                }
            }
        }

        public long DataSize {
            get {
                return ColumnSegments.Select(s => s.UsedSize).Sum();
            }
        }

        public long HierarchiesSize {
            get {
                return ColumnHierarchies.Select(h => h.UsedSize).Sum();
            }
        }

        public long TotalSize {
            get {
                return DataSize + DictionarySize + HierarchiesSize;
            }
        }
        public double? Selectivity {
            get {
                var tableRowsCount = this.Table?.RowsCount ?? 0;
                var columnCardinality = this.ColumnCardinality;
                return (columnCardinality > 0 && tableRowsCount > 0) 
                        ? ((double)columnCardinality) / ((double)tableRowsCount) 
                        : (double?)null;
            }
        }
        public List<ColumnHierarchy> ColumnHierarchies { get; }
        public List<ColumnSegment> ColumnSegments { get; }

        /// <summary>
        /// Restore references to Table object 
        /// Despite the request, Newtonsoft JSON.Net does not restore the table of the column
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            foreach (var ch in ColumnHierarchies) {
                ch.Column = this;
            }
            foreach (var cs in ColumnSegments) {
                cs.Column = this;
            }
        }
    }
}
