using System;
using System.Collections.Generic;
using System.Text;


namespace Dax.ViewModel
{
    public class VpaTablePermission
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly Dax.Metadata.TablePermission TablePermission;
#pragma warning restore IDE0052 // Remove unread private members

        internal VpaTablePermission(Dax.Metadata.TablePermission tablePermission)
        {
            this.TablePermission = tablePermission;
        }
    }
}
