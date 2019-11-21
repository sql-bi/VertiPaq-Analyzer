using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Dax.Metadata.JsonConverters
{
    public class DaxNameJsonConverter : JsonConverter<DaxName>
    {
        public override DaxName ReadJson(JsonReader reader, Type objectType, DaxName existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            // if the object has been serialized as a string the following will work
            string name = reader.Value?.ToString();

            // otherwise we need to pull out the "Name" property
            if (name == null)
            {
                JObject jo = JObject.Load(reader);
                name = (string)jo["Name"];
            }

            return new DaxName(name);
        }


        public override void WriteJson(JsonWriter writer, DaxName value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

    }
}
