using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;

namespace Dax.Metadata.JsonConverters
{   
    /// <summary>
    /// A custom creation converter for deserializing JSON into the appropriate concrete type of <see cref="CalendarColumnGroup"/>.
    /// </summary>
    /// <remarks>This converter determines the specific type of <see cref="CalendarColumnGroup"/> to
    /// instantiate based on the presence of the <c>TimeUnit</c> property in the JSON data. If the <c>TimeUnit</c>
    /// property is present, a <see cref="TimeUnitColumnAssociation"/> is created; otherwise, a <see
    /// cref="TimeRelatedColumnGroup"/> is instantiated. <para> This implementation avoids using
    /// <c>TypeNameHandling.All</c> due to security risks, as described in <see
    /// href="https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2326"> CA2326: Do not
    /// use TypeNameHandling.All</see>. Additionally, it avoids using <see cref="JsonConverter"/> directly to prevent
    /// potential stack overflow issues caused by recursive calls, as noted in <see
    /// href="https://github.com/JamesNK/Newtonsoft.Json/issues/719">GitHub issue #719</see>. </para></remarks>
    public class CalendarColumnGroupCustomCreationConverter : CustomCreationConverter<CalendarColumnGroup>
    {
        private bool _isTimeUnitColumnAssociation; // Not thread-safe

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobject = JObject.Load(reader);

            // Determine the concrete type based on the presence of the TimeUnit property used as discriminator
            _isTimeUnitColumnAssociation = jobject.TryGetValue(nameof(TimeUnitColumnAssociation.TimeUnit), StringComparison.OrdinalIgnoreCase, out _);

            return base.ReadJson(jobject.CreateReader(), objectType, existingValue, serializer);
        }

        public override CalendarColumnGroup Create(Type objectType)
        {
            if (_isTimeUnitColumnAssociation)
                return new TimeUnitColumnAssociation();

            return new TimeRelatedColumnGroup();
        }        
    }
}
