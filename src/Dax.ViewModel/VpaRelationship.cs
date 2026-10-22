using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaRelationship
    {
        private readonly Dax.Metadata.Relationship Relationship;

        internal VpaRelationship(Dax.Metadata.Relationship relationship)
        {
            this.Relationship = relationship;
        }

        public long UsedSize { get { return this.Relationship.UsedSize; } }

        public string RelationshipFromToName {
            get {
                return string.Format(
                    "{0}[{1}] {2}{3}{4} {5}[{6}]",
                    this.Relationship.FromColumn.Table.TableName.Name,
                    this.Relationship.FromColumn.ColumnName.Name,
                    this.Relationship.FromCardinalityType == null ? "<" : this.Relationship.FromCardinalityType == "Many" ? "∞" : "1",
                    this.Relationship.CrossFilteringBehavior == null ? "=" : this.Relationship.CrossFilteringBehavior == "BothDirections" ? "↔" : "←",
                    this.Relationship.ToCardinalityType == null ? "=" : this.Relationship.ToCardinalityType == "Many" ? "∞" : "1",
                    this.Relationship.ToColumn.Table.TableName.Name,
                    this.Relationship.ToColumn.ColumnName.Name
                ); ;
            }
        }

        public long FromColumnCardinality { get { return this.Relationship.FromColumn.ColumnCardinality; } }
        public long ToColumnCardinality { get { return this.Relationship.ToColumn.ColumnCardinality; } }
        public long MissingKeys { get { return this.Relationship.MissingKeys; } }
        public long InvalidRows { get { return this.Relationship.InvalidRows; } }
        /// <summary>
        /// Sample of missing keys, available only when the model was extracted in this process:
        /// the list is not saved in a VPAX file, so this is an empty string for a model read from a file.
        /// </summary>
        public string SampleReferentialIntegrityViolations { get { return string.Join(", ", this.Relationship.SampleReferentialIntegrityViolations); } }

        public bool RelyOnReferentialIntegrity { get { return this.Relationship.RelyOnReferentialIntegrity; } }
        public string JoinOnDateBehavior { get { return this.Relationship.JoinOnDateBehavior; } }
        public string CrossFilteringBehavior { get { return this.Relationship.CrossFilteringBehavior; } }
        public string RelationshipType { get { return this.Relationship.Type; } }
        public bool IsActive { get { return this.Relationship.IsActive; } }
        public string SecurityFilteringBehavior { get { return this.Relationship.SecurityFilteringBehavior; } }

        /// <summary>
        /// The ratio between the cardinality of the To column and the rows of the From table, 0 when the From table has no rows.
        /// It is computed for every relationship, but it is meaningful only for 1:M relationships, and it can be greater than 1.
        /// </summary>
        public double OneToManyRatio { get { return (this.Relationship.FromColumn.Table.RowsCount == 0) ? 0 : (double)this.Relationship.ToColumn.ColumnCardinality / (double)this.Relationship.FromColumn.Table.RowsCount; } }

        public string FromColumnName => $"'{Relationship.FromColumn.Table.TableName.Name}'[{Relationship.FromColumn.ColumnName.Name}]";
        public string ToColumnName => $"'{Relationship.ToColumn.Table.TableName.Name}'[{Relationship.ToColumn.ColumnName.Name}]";
    }
}
