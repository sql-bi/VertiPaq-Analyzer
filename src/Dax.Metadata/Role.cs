using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Dax.Metadata
{
    [DebuggerDisplay("{RoleName.Name,nq}")]
    public class Role
    {
        [JsonIgnore]
        public Model Model;

        public DaxName RoleName { get; set; }
        public List<TablePermission> TablePermissions { get; }

        public Role(Model model) : this()
        {
            Model = model;
        }

        private Role() { 
            TablePermissions = new List<TablePermission>();
        }
       
        /// <summary>
        /// Restore references to Role object 
        /// Despite the request, Newtonsoft JSON.Net does not restore the object owner of the array
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            foreach (var tp in TablePermissions) {
                tp.Role = this;
            }
        }
        
    }
}
