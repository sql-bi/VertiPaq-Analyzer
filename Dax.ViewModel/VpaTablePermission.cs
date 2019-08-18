using System;
using System.Collections.Generic;
using System.Text;


namespace Dax.ViewModel
{
    public class VpaTablePermission
    {
        private Dax.Model.TablePermission TablePermission;

        internal VpaTablePermission(Dax.Model.TablePermission tablePermission)
        {
            this.TablePermission = tablePermission;
        }
    }
}
