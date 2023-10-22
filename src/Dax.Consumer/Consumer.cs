using Dax.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dax.Tcdx.Metadata;

namespace Dax.Consumer
{
    public class Consumer
    {
        public Consumer() 
        {
            this.Items = new List<Item>();
            this.ConsumerProperties = new Dictionary<string, TcdxName>();   
        }
        
        public Dictionary<string, TcdxName> ConsumerProperties { get; set; }
        //public TcdxName TcdxName { get; set; }

        //// TODO: should we add canonical names or an enum type?
        //public string ConsumerType { get; set; }

        public List<Item> Items { get; set; }

        public IEnumerable<ModelDependency> GetModels()
        {
            return
                (from i in Items
                 select i.Model).Distinct();
        }
    }
}
