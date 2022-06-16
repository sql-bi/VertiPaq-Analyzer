using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Metadata
{
    public class Partition
    {
        public enum PartitionState
        {
            Read = 1,
            NoData = 3,
            CalculationNeeded = 4,
            SemanticError = 5,
            EvaluationError = 6,
            DependencyError = 7,
            Incomplete = 8,
            SyntaxError = 9,
            ForceCalculationNeeded = 10
        }

        public enum PartitionType
        {
            Query = 1,
            Calculated = 2,
            None = 3,
            M = 4,
            Entity = 5,
            PolicyRange = 6,
            CalculationGroup = 7,
            Inferred = 8
        }

        public enum PartitionMode
        {
            Import = 0,
            DirectQuery = 1,
            Default = 2,
            Push = 3,
            Dual = 4
        }

        public Partition(Table table)
        {
            Table = table;
        }
        private Partition() { }

        public Table Table { get; set; }
        public DaxName PartitionName { get; set; }
        public long PartitionNumber { get; set; }

        public DaxNote Description { get; set; }

        public PartitionState? State { get; set; }
        public PartitionType? Type { get; set; }
        public PartitionMode? Mode { get; set; }
        public DateTime? RefreshedTime { get; set; }
    }
}
