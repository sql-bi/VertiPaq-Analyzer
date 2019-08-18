using System;
using System.Collections.Generic;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaRelationship
    {
        private Dax.Model.Relationship Relationship;

        internal VpaRelationship(Dax.Model.Relationship relationship)
        {
            this.Relationship = relationship;
        }

        public long UsedSize { get { return this.Relationship.UsedSize; } }

        public string RelationshipFromToName {
            get {
                return string.Format(
                    "'{0}'[{1}] -> '{2}[{3}]",
                    this.Relationship.FromColumn.Table.TableName.Name,
                    this.Relationship.FromColumn.ColumnName.Name,
                    this.Relationship.ToColumn.Table.TableName.Name,
                    this.Relationship.ToColumn.ColumnName.Name
                );
            }
        }

        public long FromColumnCardinality { get { return this.Relationship.FromColumn.ColumnCardinality; } }
        public long ToColumnCardinality { get { return this.Relationship.ToColumn.ColumnCardinality; } }
    }
}
