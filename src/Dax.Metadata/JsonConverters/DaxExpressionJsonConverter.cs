using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Dax.Metadata.JsonConverters
{
    public class DaxExpressionJsonConverter : JsonConverter<DaxExpression>
    {
        public override DaxExpression ReadJson(JsonReader reader, Type objectType, DaxExpression existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            /* For future implementation
             * if DaxName is implemented with encryption/tokenization, then we might use
             * something similar to the following code
             */
            //string name;
            //if (reader.Value is JObject) {
            //    JObject jo = JObject.Load(reader);
            //    name = (string)jo["Expression"];
            //}
            //else {
            //    // if the object has been serialized as a string the following will work
            //    name = reader.Value.ToString();
            //}
            //return new DaxExpression(name);

            string name = reader.Value.ToString();
            return new DaxExpression(name);
        }


        public override void WriteJson(JsonWriter writer, DaxExpression value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Expression);
        }

    }
}
