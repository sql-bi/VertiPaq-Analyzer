using Dax.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Tcdx.Metadata
{
    public class QueryGroupsCollection 
    {
        public QueryGroupsCollection ()
        {
            this.QueryGroups = new List<QueryGroup>();
            this.QueryGroupsCollectionProperties = new Dictionary<string, TcdxName>();
        }

        public Dictionary<string, TcdxName> QueryGroupsCollectionProperties { get; set; }

        public List<QueryGroup> QueryGroups { get; set; }
    }
}
