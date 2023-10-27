using Dax.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Tcdx.Metadata
{
    enum EnumConsumerType
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
        
        EnumConsumerType ConsumerType { get; set; }
        TcdxName HostName { get; set; }
        TcdxName Container { get; set;}
        TcdxName FileName { get; set; }
        TcdxName Uri { get; set; }
        DateTime UtcAcquisition { get; set; }
        DateTime UtcModification { get; set; }

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
