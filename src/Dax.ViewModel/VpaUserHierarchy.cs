using System;
using System.Collections.Generic;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaUserHierarchy
    {
        private Dax.Metadata.UserHierarchy UserHierarchy;

        internal VpaUserHierarchy(Dax.Metadata.UserHierarchy userHierarchy)
        {
            this.UserHierarchy = userHierarchy;
        }
    }
}
