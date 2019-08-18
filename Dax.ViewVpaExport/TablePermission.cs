using System;
using System.Linq;
using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class TablePermission
    {
        [JsonIgnore]
        private Dax.Model.TablePermission _TablePermission;

        internal TablePermission(Dax.Model.TablePermission tablePermission)
        {
            this._TablePermission = tablePermission;
        }

        public string RoleName { get { return this._TablePermission.Role.RoleName.Name; } }
        public string TableName { get { return this._TablePermission.Table.TableName.Name; } }
        public string FilterExpression { get { return this._TablePermission.FilterExpression.Expression; } }

    }
}
