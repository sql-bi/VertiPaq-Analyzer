using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dax.Metadata;

namespace Dax.Tcdx.Metadata
{
    public class TableDependency
    {
        public TableDependency()
        {
            // we initialize the Model dependency to a dummy value to avoid null references
            this.Model = ModelDependency._dummyModelDependency;
        }

        public ModelDependency Model { get; set; }
        public TcdxName TableName { get; set; }

        // Connection to model table if available
        public Table ModelTable { get; set; }
    }
}
