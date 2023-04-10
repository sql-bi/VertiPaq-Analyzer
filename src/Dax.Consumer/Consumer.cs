using Dax.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Consumer
{
    public class Consumer
    {
        public Consumer() 
        {
            this.Items = new List<Item>();
        }
        
        public DaxName ConsumerName { get; set; }

        // TODO: should we add canonical names or an enum type?
        public string ConsumerType { get; set; }

        public List<Item> Items { get; set; }

        public IEnumerable<ModelDependency> GetModels()
        {
            return
                (from i in Items
                 select i.Model).Distinct();
        }
    }
}
