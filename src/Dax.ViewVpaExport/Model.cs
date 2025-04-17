using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class Model
    {
        [JsonIgnore]
        private readonly Dax.Metadata.Model _Model;
        public IEnumerable<Table> Tables { get { return from t in this._Model.Tables select new Table(t); } }
        public IEnumerable<Column> Columns {
            get {
                return
                    from t in this._Model.Tables
                    from c in t.Columns
                    select new Column(c);
            }
        }
        public IEnumerable<Measure> Measures {
            get {
                return
                    from t in this._Model.Tables
                    from m in t.Measures
                    select new Measure(m);
            }
        }

        public IEnumerable<ColumnSegment> ColumnsSegments {
            get {
                return
                    from t in this._Model.Tables
                    from c in t.Columns
                    from cs in c.ColumnSegments
                    select new ColumnSegment(cs);
            }
        }

        public IEnumerable<ColumnHierarchy> ColumnsHierarchies {
            get {
                return
                    from t in this._Model.Tables
                    from c in t.Columns
                    from ch in c.ColumnHierarchies
                    select new ColumnHierarchy(ch);
            }
        }

        public IEnumerable<UserHierarchy> UserHierarchies {
            get {
                return
                    from t in this._Model.Tables
                    from uh in t.UserHierarchies
                    select new UserHierarchy(uh);
            }
        }

        public IEnumerable<Relationship> Relationships {
            get {
                return
                    from r in this._Model.Relationships
                    select new Relationship(r);
            }
        }

        public IEnumerable<TablePermission> TablePermissions {
            get {
                return
                    from r in this._Model.Roles
                    from tp in r.TablePermissions
                    select new TablePermission(tp);
            }
        }

        public IEnumerable<CalculationGroup> CalculationGroups {
            get {
                return
                    from t in this._Model.Tables
                    where t.CalculationGroup != null
                    select new CalculationGroup(t.CalculationGroup);
            }
        }

        public IEnumerable<CalculationItem> CalculationItems {
            get {
                return
                    from t in this._Model.Tables
                    where t.CalculationGroup != null
                    from item in t.CalculationGroup.CalculationItems
                    select new CalculationItem(item);
            }
        }
        
        public Model(Dax.Metadata.Model model)
        {
            this._Model = model;
        }
    }
}
