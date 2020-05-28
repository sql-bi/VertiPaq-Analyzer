using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dax.Metadata
{
    public class Relationship
    {
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
        public long MissingKeys { get; set; }
        public long InvalidRows { get; set; }

        /// <summary>
        /// The sample of referential integrity violations is not persisted 
        /// to avoid exposing sensitive data
        /// The list is available only to the tool that accesses the database online
        /// </summary>
        [JsonIgnore]
        public List<string> SampleReferentialIntegrityViolations { get; set; }

        [JsonIgnore]
        public long UsedSize {
            get {
                return UsedSizeFrom + UsedSizeTo;
            }
        }

        public Relationship( Column fromColumn, Column toColumn ) : this() {
            FromColumn = fromColumn;
            ToColumn = toColumn;
        }
        private Relationship() {
            SampleReferentialIntegrityViolations = new List<string>();
        }

        [JsonIgnore]
        public int Dmv1200RelationshipId;
    }
}
