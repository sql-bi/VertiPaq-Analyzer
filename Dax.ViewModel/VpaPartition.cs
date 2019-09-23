using System;
using System.Collections.Generic;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaPartition
    {
        private Dax.Metadata.Partition Partition;

        internal VpaPartition(Dax.Metadata.Partition partition)
        {
            this.Partition = partition;
        }
    }
}
