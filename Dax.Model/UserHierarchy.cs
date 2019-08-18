using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Model
{
    public class UserHierarchy
    {
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
