using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dax.Metadata;

namespace Dax.Consumer
{
    public class Item
    {
        public Item() 
        { 
            this.TableDependencies = new List<TableDependency>(); 
            this.ColumnDependencies = new List<ColumnDependency>(); 
            this.MeasureDependencies = new List<MeasureDependency>();   
        }

        public DaxName ItemName { get; set; }

        // TODO: should we add canonical names or an enum type?
        public string ItemType { get; set; }

        // Query language could be DAX / MDX / SQL?? / Undefined
        public string QueryLanguage { get; set; }

        public ModelDependency Model { get; set; }
        public List<TableDependency> TableDependencies { get; set; }
        public List<ColumnDependency> ColumnDependencies { get; set; }
        public List<MeasureDependency> MeasureDependencies { get; set; }
    }
}
