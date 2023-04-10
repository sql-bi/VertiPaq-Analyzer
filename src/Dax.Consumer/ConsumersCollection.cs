using Dax.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Consumer
{
    public class ConsumersCollection : List<Consumer>
    {
        public DaxName Path { get; set; }
    }
}
