using Dax.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Tcdx.Metadata
{
    public class ConsumersCollection 
    {
        public ConsumersCollection ()
        {
            this.Consumers = new List<Consumer>();
            this.ConsumersCollectionProperties = new Dictionary<string, TcdxName>();
        }

        /// <summary>
        /// this will contain the properties of the collection, that for instance can be the path of the
        /// folder containing the files, each one representing a consumer
        /// the string is the name of the property, the TcdxName is the value of the property
        /// </summary>
        public Dictionary<string, TcdxName> ConsumersCollectionProperties { get; set; }

        List<Consumer> Consumers { get; set; }
    }
}
