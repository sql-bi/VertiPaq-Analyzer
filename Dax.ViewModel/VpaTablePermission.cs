using System;
using System.Collections.Generic;
using System.Text;


namespace Dax.ViewModel
{
    public class VpaTablePermission
    {
        private Dax.Metadata.TablePermission TablePermission;

        internal VpaTablePermission(Dax.Metadata.TablePermission tablePermission)
        {
            this.TablePermission = tablePermission;
        }
    }
}
