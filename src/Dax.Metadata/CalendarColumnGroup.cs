using Dax.Metadata.JsonConverters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dax.Metadata
{
    [JsonConverter(typeof(CalendarColumnGroupCustomCreationConverter))]
    public abstract class CalendarColumnGroup
    {
        [JsonIgnore]
        public Calendar Calendar { get; set; }
    }

    public class TimeRelatedColumnGroup : CalendarColumnGroup
    {
        public List<Column> Columns { get; set; } = [];
    }

    public class TimeUnitColumnAssociation : CalendarColumnGroup
    {
        public List<Column> AssociatedColumns { get; set; } = [];

        public Column PrimaryColumn { get; set; }

        public TimeUnit TimeUnit { get; set; }
    }
}
