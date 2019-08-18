using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dax.Model
{
    public class Relationship
    {
        private Dax.Model.Relationship _Relationship;

        internal Relationship(Dax.Model.Relationship relationship)
        {
            this._Relationship = relationship;
        }

        public Column FromColumn { get; }
        public string FromCardinalityType { get; set; }
        public Column ToColumn { get; }
        public string ToCardinalityType { get; set; }
        public bool RelyOnReferentialIntegrity { get; set; }
        public string JoinOnDateBehavior { get; set; }
        public string CrossFilteringBehavior { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string SecurityFilteringBehavior { get; set; }

        public long UsedSizeFrom { get; set; }
        public long UsedSizeTo { get; set; }
        [JsonIgnore]
        public long UsedSize {
            get {
                return UsedSizeFrom + UsedSizeTo;
            }
        }

        public Relationship( Column fromColumn, Column toColumn ) {
            FromColumn = fromColumn;
            ToColumn = toColumn;
        }
        private Relationship() { }

        [JsonIgnore]
        public int Dmv1200RelationshipId;
    }
}
