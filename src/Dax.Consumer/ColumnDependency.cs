using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dax.Metadata;

namespace Dax.Consumer
{
    public class ColumnDependency
    {
        public ModelDependency Model { get; set; }

        public DaxName TableName { get; set; }
        public DaxName ColumnName { get; set; }

        public TableDependency Table { get; set; }

        // Connection to model column if available
        public Column ModelColumn { get; set; }
    }
}
