using System;
using System.Collections.Generic;
using System.Text;

namespace Dax.ViewModel
{
    public class VpaUserHierarchy
    {
        private Dax.Model.UserHierarchy UserHierarchy;

        internal VpaUserHierarchy(Dax.Model.UserHierarchy userHierarchy)
        {
            this.UserHierarchy = userHierarchy;
        }
    }
}
