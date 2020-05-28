using System;
using System.Linq;
using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class Relationship
    {
        [JsonIgnore]
        private readonly Dax.Metadata.Relationship _Relationship;

        internal Relationship(Dax.Metadata.Relationship relationship)
        {
            this._Relationship = relationship;
        }

        public string FromTableName {
            get {
                return this._Relationship.FromColumn.Table.TableName.Name;
            }
        }
        public string FromFullColumnName {
            get {
                return Column.GetFullColumnName(this._Relationship.FromColumn);
            }
        }
        public long FromCardinality { get { return this._Relationship.FromColumn.ColumnCardinality; } }
        public string FromCardinalityType { get { return this._Relationship.FromCardinalityType; } }


        public string ToTableName {
            get {
                return this._Relationship.ToColumn.Table.TableName.Name;
            }
        }
        public string ToFullColumnName {
            get {
                return Column.GetFullColumnName(this._Relationship.ToColumn);
            }
        }
        public long ToCardinality { get { return this._Relationship.ToColumn.ColumnCardinality; } }
        public string ToCardinalityType { get { return this._Relationship.ToCardinalityType; } }
        public bool RelyOnReferentialIntegrity { get { return this._Relationship.RelyOnReferentialIntegrity; } }
        public string JoinOnDateBehavior { get { return this._Relationship.JoinOnDateBehavior; } }
        public string CrossFilteringBehavior { get { return this._Relationship.CrossFilteringBehavior; } }
        public string RelationshipType { get { return this._Relationship.Type; } }
        public bool IsActive { get { return this._Relationship.IsActive; } }
        public string RelationshipName { get { return this._Relationship.Name; } }
        public string SecurityFilteringBehavior { get { return this._Relationship.SecurityFilteringBehavior; } }

        public long UsedSizeFrom { get { return this._Relationship.UsedSizeFrom; } }
        public long UsedSizeTo { get { return this._Relationship.UsedSizeTo; } }
        public long UsedSize { get { return this._Relationship.UsedSize; } }
        public long MissingKeys { get { return this._Relationship.MissingKeys; } }
        public long InvalidRows { get { return this._Relationship.InvalidRows; } }
    }
}
