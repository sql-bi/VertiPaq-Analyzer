using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dax.Metadata;

namespace Dax.Consumer
{
    public class ModelDependency
    {
        public DaxName ServerName { get; set; }
        public DaxName ModelName { get; set; }

        // Connection to related model if available
        public Model Model { get; set; }
    }
}
