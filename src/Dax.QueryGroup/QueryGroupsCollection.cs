using Dax.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dax.Tcdx.Metadata;

namespace Dax.QueryGroup
{
    public class QueryGroupsCollection 
    {
        public QueryGroupsCollection ()
        {
            this.QueryGroups = new List<QueryGroup>();
            this.QueryGroupsCollectionProperties = new Dictionary<string, TcdxName>();
        }

        public Dictionary<string, TcdxName> QueryGroupsCollectionProperties { get; set; }

        List<QueryGroup> QueryGroups { get; set; }
    }
}
