using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Dax.Tcdx.Metadata.JsonConverters
{

    public class TcdxNameJsonConverter : JsonConverter<TcdxName>
    {
        public override TcdxName ReadJson(JsonReader reader, Type objectType, TcdxName existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var jobject = JObject.Load(reader);
                var nameValue = (string)jobject["Name"];
                return new TcdxName(nameValue);
            }

            string name = (string)reader.Value;
            return new TcdxName(name);
        }

        public override void WriteJson(JsonWriter writer, TcdxName value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }
    }
}
