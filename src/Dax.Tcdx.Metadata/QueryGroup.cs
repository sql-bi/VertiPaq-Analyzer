using Dax.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Dax.Tcdx.Metadata
{
    public enum EnumQueryGroupType
    {
        QueryAnalytics,
        Profiler,
        ExtendedEvents,
        OtherQueryGroupType
    }
    public class QueryGroup
    {
        public QueryGroup()
        {
            this.QueryGroupProperties = new Dictionary<string, TcdxName>();
            this.Model = ModelDependency._dummyModelDependency;
            this.TableQueries = new Dictionary<string, int>();
            this.ColumnQueries = new Dictionary<string, int>();
            this.MeasureQueries = new Dictionary<string, int>();
            this.TokenQueries = new Dictionary<string, int>();
            this.QueryGroupType = EnumQueryGroupType.OtherQueryGroupType;
        }

        /// <summary>
        /// the correlation can be used to link the qeury group to a separate log file containing the queries, that are not to be 
        /// included into the tcdx file for privacy reasons
        /// </summary>
        public string CorrelationId { get; set; }

        public EnumQueryGroupType QueryGroupType { get; set; }

        public TcdxName QueryGroupName { get; set; }


        public Dictionary<string, TcdxName> QueryGroupProperties { get; set; }

        // this reference should not be null, but in case it is, it should be replaced by ModelDependency._dummyModelDependency
        public ModelDependency Model { get; set; }

        // the Item can be null
        public Item Item { get; set; }

        // counters of numbner of queries where the table, column, measure or token are referred
        public Dictionary<string, int> TableQueries { get; set; }
        public Dictionary<string, int> ColumnQueries { get; set; }
        public Dictionary<string, int> MeasureQueries { get; set; }

        // token can be a column or a measure but it was not possible to distinguish them from the context
        public Dictionary<string, int> TokenQueries { get; set; }

        public long TotalExecTimeMilliseconds { get; set; }
        public int NumberOfQueries { get; set; }
        public int MaxExcTimeMilliseconds { get; set; }
        public int MinExcTimeMilliseconds { get; set; }
        public int AverageExcTimeMilliseconds { get; set; }
        public double StandardDeviationExcTimeMilliseconds { get; set; }

        // consider using DateTimeOffset instead of DateTime, to include the timezone
        public DateTime UtcStart { get; set; }
        public DateTime UtcEnd { get; set; }
    }
}
