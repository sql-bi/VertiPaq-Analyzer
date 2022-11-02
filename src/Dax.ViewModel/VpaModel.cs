using System;
using System.Collections.Generic;
using System.Linq;

namespace Dax.ViewModel
{
    public class VpaModel
    {
        public Dax.Metadata.Model Model { get; private set; }
        public IEnumerable<VpaTable> Tables { get { return from t in this.Model.Tables select new VpaTable( t ); } }
        public IEnumerable<VpaColumn> Columns {
            get {
                return
                    from t in this.Model.Tables
                    from c in t.Columns
                    select new VpaColumn(c);
            }
        }
        public IEnumerable<VpaRelationship> Relationships { get { return from r in this.Model.Relationships select new VpaRelationship(r); } }
        public IEnumerable<VpaTable> TablesWithFromRelationships {
            get {
                return
                    from t in this.Model.Tables
                    where t.GetRelationshipsFrom().Any()
                    select new VpaTable(t);
            }
        }
        /*
        public List<VpaRole> Roles { get; }
        */
        public VpaModel( Dax.Metadata.Model model )
        {
            this.Model = model;
        }
    }
}
