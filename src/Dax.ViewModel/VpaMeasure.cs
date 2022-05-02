using System;
using System.Collections.Generic;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaMeasure
    {
        private readonly Dax.Metadata.Measure Measure;

        internal VpaMeasure(Dax.Metadata.Measure measure)
        {
            this.Measure = measure;
        }

        public bool IsReferenced { get { return this.Measure.IsReferenced; } }
    }
}