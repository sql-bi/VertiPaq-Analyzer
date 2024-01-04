using Dax.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Tcdx.Metadata
{
    public enum EnumConsumerType
    {
        Excel,
        PowerBIService,
        PowerBIDesktop,
        OtherConsumerType
    }

    public class Consumer
    {
        public Consumer() 
        {
            this.Items = new List<Item>();
            this.ConsumerProperties = new Dictionary<string, TcdxName>();   
            this.ConsumerType = EnumConsumerType.OtherConsumerType;
        }
        
        public EnumConsumerType ConsumerType { get; set; }
        public TcdxName HostName { get; set; }
        public TcdxName Container { get; set;}
        public TcdxName FileName { get; set; }
        public TcdxName Uri { get; set; }
        public DateTime UtcAcquisition { get; set; }
        public DateTime UtcModification { get; set; }

        public Dictionary<string, TcdxName> ConsumerProperties { get; set; }
        public List<Item> Items { get; set; }

        public IEnumerable<ModelDependency> GetModels()
        {
            return
                (from i in Items
                 select i.Model).Distinct();
        }
    }
}
