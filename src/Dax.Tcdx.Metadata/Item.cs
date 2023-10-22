using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dax.Metadata;

namespace Dax.Tcdx.Metadata
{

    /*

            usare un dizionario al posto dell'itemname con chiave / valore dove valore e' una classe come TcdxName che pero' si chiama DaxConsumer
            sta roba inglobera' anche ItemType che e' una delle n. proprieta
            la chiave la lasciamo visibile mentre TcdxName si puo' nascondere
    

            stesso per Consumer dove pero' manteniamo ConsumerType
     */
    public class Item
    {
        public Item() 
        { 
            this.TableDependencies = new List<TableDependency>(); 
            this.ColumnDependencies = new List<ColumnDependency>(); 
            this.MeasureDependencies = new List<MeasureDependency>();
            this.ItemProperties = new Dictionary<string, TcdxName>();
            // we set the Model to a dummy model dependency to avoid null references
            this.Model = ModelDependency._dummyModelDependency;
        }

        public Dictionary<string, TcdxName> ItemProperties  { get; set; }

        // the ItemType became a proprerty and will be found in the ItemProperties dictionary
        // public string ItemType { get; set; }

        // Query language could be DAX / MDX / SQL?? / Undefined
        // this also become a property in the ItemProperties dictionary
        // public string QueryLanguage { get; set; }

        public ModelDependency Model { get; set; }
        public List<TableDependency> TableDependencies { get; set; }
        public List<ColumnDependency> ColumnDependencies { get; set; }
        public List<MeasureDependency> MeasureDependencies { get; set; }
    }
}
