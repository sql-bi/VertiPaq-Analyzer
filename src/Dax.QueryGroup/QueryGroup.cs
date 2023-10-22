using Dax.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dax.Tcdx.Metadata;
using System.Numerics;

namespace Dax.QueryGroup
{
    public class QueryGroup
    {
        public QueryGroup() 
        {
            this.QueryGroupProperties = new Dictionary<string, TcdxName>();   
            this.Model = ModelDependency._dummyModelDependency;
            this.TableQueries = new Dictionary<TcdxName, int>();
            this.ColumnQueries = new Dictionary<TcdxName, int>();
            this.MeasureQueries = new Dictionary<TcdxName, int>();
            this.TokenQueries = new Dictionary<TcdxName, int>();

        }

        /// <summary>
        /// the correlation can be used to link the qeury group to a separate log file containing the queries, that are not to be 
        /// included into the tcdx file for privacy reasons
        /// </summary>
        string CorrelationId { get; set; }

        public Dictionary<string, TcdxName> QueryGroupProperties { get; set; }

        // this reference should not be null, but in case it is, it should be replaced by ModelDependency._dummyModelDependency
        public ModelDependency Model { get; set; }

        // the Item can be null
        public Item Item { get; set; }

        // counters of numbner of queries where the table, column, measure or token are referred
        public Dictionary<TcdxName, int> TableQueries { get; set; }
        public Dictionary<TcdxName, int> ColumnQueries { get; set; }
        public Dictionary<TcdxName, int> MeasureQueries { get; set; }

        // token can be a column or a measure but it was not possible to distinguish them from the context
        public Dictionary<TcdxName, int> TokenQueries{ get; set; }

        public Int64 TotalExecTimeMilliseconds { get; set; }
        public Int32 NumberOfQueries { get; set; }
        public Int32 MaxExcTimeMilliseconds { get; set; }
        public Int32 MinExcTimeMilliseconds { get; set; }
        public Int32 AverageExcTimeMilliseconds { get; set; }
        public double StandardDeviationExcTimeMilliseconds { get; set; }

        // consider using DateTimeOffset instead of DateTime, to include the timezone
        public DateTime UtcStart { get; set; }
        public DateTime UtcEnd { get; set; }
    }
}
