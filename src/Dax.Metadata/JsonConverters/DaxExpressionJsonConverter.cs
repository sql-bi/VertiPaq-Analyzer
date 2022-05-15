using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Dax.Metadata.JsonConverters
{
    public class DaxExpressionJsonConverter : JsonConverter<DaxExpression>
    {
        public override DaxExpression ReadJson(JsonReader reader, Type objectType, DaxExpression existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // This "if" statement handles DaxExpression deserialization for older VPAX files
            // where DaxExpression properties were serialized as a JObject instead of string.

            if (reader.TokenType == JsonToken.StartObject) 
            {
                var jobject = JObject.Load(reader);
                var expressionValue = (string)jobject["Expression"];
                return new DaxExpression(expressionValue);
            }

            string expression = (string)reader.Value;
            return new DaxExpression(expression);
        }

        public override void WriteJson(JsonWriter writer, DaxExpression value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Expression);
        }
    }
}
