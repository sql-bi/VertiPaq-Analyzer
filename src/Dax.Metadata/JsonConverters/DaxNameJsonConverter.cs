using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Dax.Metadata.JsonConverters
{
    public class DaxNameJsonConverter : JsonConverter<DaxName>
    {
        public override DaxName ReadJson(JsonReader reader, Type objectType, DaxName existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // This "if" statement handles DaxName deserialization for older VPAX files
            // where DaxNode properties were serialized as a JObject instead of string.

            if (reader.TokenType == JsonToken.StartObject)
            {
                var jobject = JObject.Load(reader);
                var nameValue = (string)jobject["Name"];
                return new DaxName(nameValue);
            }

            string name = (string)reader.Value;
            return new DaxName(name);
        }

        public override void WriteJson(JsonWriter writer, DaxName value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }
    }
}
