using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dax.Metadata
{
    [DebuggerDisplay("{HierarchyName.Name,nq}")]
    public class UserHierarchy
    {
        [JsonIgnore]
        public Table Table { get; set; }
        public DaxName HierarchyName { get; set; }
        public bool IsHidden { get; set; }
        public long UsedSize { get; set; }

        public List<Column> Levels { get; }

        public UserHierarchy(Table table) : this()
        {
            Table = table;
        }

        public UserHierarchy ()
        {
            Levels = new List<Column>();
        }
    }
}
