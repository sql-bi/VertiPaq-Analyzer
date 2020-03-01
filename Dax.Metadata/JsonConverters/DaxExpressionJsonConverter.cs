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

            // if the object has been serialized as a string the following will work
            string expression = reader.Value?.ToString();

            // otherwise we need to pull out the "Expression" property
            if (expression == null)
            {
                JObject jo = JObject.Load(reader);
                expression = (string)jo["Expression"];
            }

            return new DaxExpression(expression);
        }


        public override void WriteJson(JsonWriter writer, DaxExpression value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Expression);
        }

    }
}
