using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Dax.Metadata.JsonConverters
{
    public class DaxNoteJsonConverter : JsonConverter<DaxNote>
    {
        public override DaxNote ReadJson(JsonReader reader, Type objectType, DaxNote existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // This "if" statement is a workaround to handle the deserialization of the DaxNode class in versions
            // prior to 1.2.5 where the "DaxNoteJsonConverter" attribute was not applied to the "DaxNode" class.
            // In those versions all properties of type DaxNode were serialized as JObject instead of string type

            if (reader.TokenType == JsonToken.StartObject) 
            {
                var jobject = JObject.Load(reader);
                var noteValue = (string)jobject["Note"];
                return new DaxNote(noteValue);
            }

            string note = (string)reader.Value;
            return new DaxNote(note);
        }

        public override void WriteJson(JsonWriter writer, DaxNote value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Note);
        }
    }
}
