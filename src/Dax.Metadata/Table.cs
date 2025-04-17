using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Dax.Metadata
{
    [DebuggerDisplay("{TableName.Name,nq}")]
    [JsonObject(IsReference =true)]
    public class Table
    {
        public enum TableSourceType {
            DataSource, // not used in serialization, use null instead
            CalculatedTable,
            CalculationGroup
        }
        public Table(Model model) : this()
        {
            Model = model;
        }

        private Table() { 
            Columns = new List<Column>();
            Measures = new List<Measure>();
            UserHierarchies = new List<UserHierarchy>();
            Partitions = new List<Partition>();
        }

        [JsonIgnore]
        public Model Model { get; set; }

        public DaxName TableName { get; set; }
        public string TableType { get; set; }
        public bool IsHidden { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsLocalDateTable { get; set; }
        public bool IsTemplateDateTable { get; set; }
        public DaxExpression TableExpression { get; set; }
        public DaxExpression DefaultDetailRowsExpression { get; set; }
        public long RowsCount { get; set; }
        public long ReferentialIntegrityViolationCount { get; set; }
        public CalculationGroup CalculationGroup { get; set; }
        public DaxNote Description { get; set; }
        public bool IsReferenced { get; set; }

        public bool HasDualPartitions {
            get {
                foreach (var partition in Partitions) {
                    if (partition.Mode == Partition.PartitionMode.Dual) 
                        return true;
                    if (partition.Mode == Partition.PartitionMode.Default && this.Model.DefaultMode == Partition.PartitionMode.Dual) 
                        return true;
                }
                return false;
            }
        }
        public bool HasDirectQueryPartitions { 
            get {
                foreach (var partition in Partitions) {
                    if (partition.Mode == Partition.PartitionMode.DirectQuery || partition.Mode == Partition.PartitionMode.Dual)
                        return true;
                    if (partition.Mode == Partition.PartitionMode.Default && (this.Model.DefaultMode == Partition.PartitionMode.DirectQuery || this.Model.DefaultMode == Partition.PartitionMode.Dual))
                        return true;
                }
                return false;
            }
        }

        public bool HasDirectLakePartitions {
            get {
                foreach (var partition in Partitions) {
                    if (partition.Mode == Partition.PartitionMode.DirectLake)
                        return true;
                    if (partition.Mode == Partition.PartitionMode.Default && (this.Model.DefaultMode == Partition.PartitionMode.DirectLake))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Returns true if the table is marked as auto-date/time table or is a valid user-defined date/time table, otherwise it returns false
        /// </summary>
        /// <remarks>
        /// If the value is null, it means that the model does not contain this information (e.g. the vpax file was extracted with an earlier version of this library that did not support this property)
        /// </remarks>
        public bool? IsDateTable { get; set; }

        [JsonIgnore]
        public long ColumnsDictionarySize { get {
                return this.Columns.Sum(c => c.DictionarySize);
            }
        }

        [JsonIgnore]
        public long ColumnsDataSize {
            get {
                return this.Columns.Sum(c => c.DataSize);
            }
        }

        [JsonIgnore]
        public long ColumnsHierarchiesSize {
            get {
                return this.Columns.Sum(c => c.HierarchiesSize);
            }
        }

        [JsonIgnore]
        public long ColumnsTotalSize {
            get {
                return this.ColumnsDataSize + this.ColumnsDictionarySize + this.ColumnsHierarchiesSize; 
            }
        }

        [JsonIgnore]
        public long TableSize {
            get {
                return this.ColumnsTotalSize + this.UserHierarchiesSize + this.RelationshipsSize; 
            }
        }

        [JsonIgnore]
        public long RelationshipsFromSize { get { return GetRelationshipsFrom().Sum(r => r.UsedSizeFrom); } }

        [JsonIgnore]
        public long RelationshipsToSize { get { return GetRelationshipsTo().Sum(r => r.UsedSizeTo); } }

        [JsonIgnore]
        public long RelationshipsSize {
            get {
                return RelationshipsFromSize + RelationshipsToSize;
            }
        }

        [JsonIgnore]
        public long UserHierarchiesSize {
            get {
                return
                    this.UserHierarchies.Sum(uh => uh.UsedSize);
            }
        }
        // TODO - move to DaxName or similar class if we want to serialize these properties
        [JsonIgnore]
        public string Dmv1100TableId { get; private set; }
        [JsonIgnore]
        public int Dmv1200TableId { get; private set; }

        public void SetDmv1100TableId(string dmv1100Id)
        {
            Dmv1100TableId = dmv1100Id;
            var openIdBracket = dmv1100Id.LastIndexOf('(');
            var closeIdBracket = dmv1100Id.LastIndexOf(')');
            if (openIdBracket >= 0 && closeIdBracket >= 0) {
                var stringDmv1200Id = dmv1100Id.Substring(openIdBracket + 1, closeIdBracket - (openIdBracket + 1));
                if (int.TryParse(stringDmv1200Id, out int intDmv1200Id)) {
                    Dmv1200TableId = intDmv1200Id;
                }
            }
        }

        public List<Column> Columns { get; }
        public List<Measure> Measures { get; }
        public List<UserHierarchy> UserHierarchies { get; }
        public List<Partition> Partitions { get; }

        public IEnumerable<TablePermission> GetTablePermissions()
        {
            if (this.Model == null) return null;

            var queryTablePermissions = 
                from role in this.Model.Roles
                from tp in role.TablePermissions
                where tp.Table.TableName == this.TableName
                select tp;
            return queryTablePermissions;
        }

        public IEnumerable<Relationship> GetRelationshipsTo()
        {
            if (this.Model == null) return null;
            var queryRelationshipsTo =
                from r in this.Model.Relationships
                where r.ToColumn.Table.TableName == this.TableName
                select r;
            return queryRelationshipsTo;
        }

        public IEnumerable<Relationship> GetRelationshipsFrom()
        {
            if (this.Model == null) return null;
            var queryRelationshipsFrom =
                from r in this.Model.Relationships
                where r.FromColumn.Table.TableName == this.TableName
                select r;
            return queryRelationshipsFrom;
        }

        /// <summary>
        /// Restore references to Table object 
        /// Despite the request, Newtonsoft JSON.Net does not restore the table of the column
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            foreach (var c in Columns) {
                c.Table = this;
            }
            foreach (var m in Measures) {
                m.Table = this;
            }
            foreach (var uh in UserHierarchies) {
                uh.Table = this;
            }
            foreach (var p in Partitions) {
                p.Table = this;
            }

            if (CalculationGroup != null)
                CalculationGroup.Table = this;
        }
    }
}
