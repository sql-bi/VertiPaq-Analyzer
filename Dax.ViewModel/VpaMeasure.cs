using System;
using System.Collections.Generic;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaMeasure
    {
        private Dax.Metadata.Measure Measure;

        internal VpaMeasure(Dax.Metadata.Measure measure)
        {
            this.Measure = measure;
        }
    }
}