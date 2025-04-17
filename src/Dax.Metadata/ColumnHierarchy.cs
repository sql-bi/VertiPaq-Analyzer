using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dax.Metadata
{
    /*
     * Original DMV
     * 
     * Ignore TABLE_NAME and STRUCTURE_NAME that should be linked to the column entity
SELECT 
    DIMENSION_NAME AS TABLE_NAME, 
    COLUMN_ID AS STRUCTURE_NAME,
    SEGMENT_NUMBER, 
    TABLE_PARTITION_NUMBER, 
    USED_SIZE,
    TABLE_ID AS COLUMN_HIERARCHY_ID
FROM $SYSTEM.DISCOVER_STORAGE_TABLE_COLUMN_SEGMENTS
WHERE LEFT ( TABLE_ID, 2 ) = 'H$'
    */
    public class ColumnHierarchy
    {
        public ColumnHierarchy(Column column)
        {
            Column = column;
        }
        private ColumnHierarchy() { }

        [JsonIgnore]
        public Column Column { get; set; }

        public DaxName StructureName { get; set; }
        public long SegmentNumber { get; set; }
        public long TablePartitionNumber { get; set; }
        public long UsedSize { get; set; }


    }
}
