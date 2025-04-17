using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Metadata
{
    public class TablePermission
    {
        [JsonIgnore]
        public Role Role { get; set; }
        public Table Table { get; set; }
        public DaxExpression FilterExpression { get; set; }

        public TablePermission ( Role role )
        {
            Role = role;
        }
        private TablePermission() { }
    }
}
