using System;
using System.Collections.Generic;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaMeasure
    {
        private Dax.Model.Measure Measure;

        internal VpaMeasure(Dax.Model.Measure measure)
        {
            this.Measure = measure;
        }
    }
}