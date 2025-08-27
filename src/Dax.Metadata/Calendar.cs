using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Dax.Metadata
{
    [DebuggerDisplay("{CalendarName.Name,nq}")]
    public class Calendar
    {
        public List<CalendarColumnGroup> CalendarColumnGroups { get; set; } = [];

        public DaxName CalendarName { get; set; }

        public DaxNote Description { get; set; }

        [JsonIgnore]
        public Table Table { get; set; }

        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            foreach (var ccg in CalendarColumnGroups)
                ccg.Calendar = this;
        }
    }
}
