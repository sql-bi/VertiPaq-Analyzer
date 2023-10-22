using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dax.Metadata;

namespace Dax.Tcdx.Metadata
{
    public class ModelDependency
    {
        // _dummyModelDependency is used to represent a model that is not available and should be used 
        // instead of leaving null references where the model dependency is expected
        public static ModelDependency _dummyModelDependency = new ModelDependency() { ModelName = new TcdxName("Dummy") };
        public TcdxName ServerName { get; set; }
        public TcdxName ModelName { get; set; }

        // Connection to related model if available
        public Model Model { get; set; }
    }
}
