using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Model
{
    public class Partition
    {
        public Partition(Table table)
        {
            Table = table;
        }
        private Partition() { }

        public Table Table { get; set; }
        public DaxName PartitionName { get; set; }
        public long PartitionNumber { get; set; }
    }
}
