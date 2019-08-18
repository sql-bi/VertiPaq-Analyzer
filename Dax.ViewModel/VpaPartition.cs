using System;
using System.Collections.Generic;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaPartition
    {
        private Dax.Model.Partition Partition;

        internal VpaPartition(Dax.Model.Partition partition)
        {
            this.Partition = partition;
        }
    }
}
