using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dax.Metadata;

namespace Dax.Consumer
{
    public class TableDependency
    {
        public ModelDependency Model { get; set; }
        public DaxName TableName { get; set; }

        // Connection to model table if available
        public Table ModelTable { get; set; }
    }
}
