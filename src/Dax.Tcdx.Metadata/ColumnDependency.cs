using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dax.Metadata;

namespace Dax.Tcdx.Metadata
{
    public class ColumnDependency
    {
        public ColumnDependency()
        {
            // we initialize the Model dependency to a dummy value to avoid null references
            this.Model = ModelDependency._dummyModelDependency;
        }

        public ModelDependency Model { get; set; }

        public TcdxName TableName { get; set; }
        public TcdxName ColumnName { get; set; }

        public TableDependency Table { get; set; }

        // Connection to model column if available
        public Column ModelColumn { get; set; }
    }
}
