using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dax.Metadata;

namespace Dax.Tcdx.Metadata
{
    public class MeasureDependency
    {
        public MeasureDependency() 
        { 
            // we initialize the model dependency to a dummy value to avoid null references
            ModelDependency._dummyModelDependency.ModelName = new TcdxName("Dummy");
        }

        public ModelDependency Model { get; set; }

        public TcdxName MeasureName { get; set; }

        // TODO: how do we manage KPIs? 

        public TableDependency Table { get; set; }

        // Connection to model measure if available

        public Measure ModelMeasure { get; set; }
    }
}
