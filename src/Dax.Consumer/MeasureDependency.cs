using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dax.Metadata;

namespace Dax.Consumer
{
    public class MeasureDependency
    {
        public ModelDependency Model { get; set; }

        public DaxName MeasureName { get; set; }

        // TODO: how do we manage KPIs? 

        public TableDependency Table { get; set; }

        // Connection to model measure if available

        public Measure ModelMeasure { get; set; }
    }
}
