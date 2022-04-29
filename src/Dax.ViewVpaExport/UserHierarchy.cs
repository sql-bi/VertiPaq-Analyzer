using System;
using System.Linq;
using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class UserHierarchy
    {
        [JsonIgnore]
        private readonly Dax.Metadata.UserHierarchy _UserHierarchy;

        internal UserHierarchy(Dax.Metadata.UserHierarchy userHierarchy)
        {
            _UserHierarchy = userHierarchy;
        }

        private UserHierarchy() { }

        public string TableName { get { return this._UserHierarchy.Table.TableName.Name; } }
        public string UserHierarchyName { get { return this._UserHierarchy.HierarchyName.Name; } }
        public bool IsHidden { get { return this._UserHierarchy.IsHidden; } }
        public long UsedSize { get { return this._UserHierarchy.UsedSize; } }
        public string Levels {
            get {
                return String.Join(", ", _UserHierarchy.Levels.Select(c => c.ColumnName.Name).ToArray());
            }
        }

    }
}
