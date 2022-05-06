using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Dax.Metadata.JsonConverters
{
    public class DaxNameJsonConverter : JsonConverter<DaxName>
    {
        public override DaxName ReadJson(JsonReader reader, Type objectType, DaxName existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // otherwise we need to pull out the "Name" property
            if (reader.TokenType == JsonToken.Null) return null;

            /* For future implementation
             * if DaxName is implemented with encryption/tokenization, then we might use
             * something similar to the following code
             */
            //string name;
            //if (reader.Value is JObject) {
            //    JObject jo = JObject.Load(reader);
            //    name = (string)jo["Name"];
            //}
            //else {
            //    // if the object has been serialized as a string the following will work
            //    name = reader.Value.ToString();
            //}
            //return new DaxName(name);

            string name = reader.Value.ToString();
            return new DaxName(name);
        }


        public override void WriteJson(JsonWriter writer, DaxName value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

    }
}
