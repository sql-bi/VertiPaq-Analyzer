using Dax.Metadata.JsonConverters;
using Newtonsoft.Json;

namespace Dax.Metadata
{
    /// <summary>
    /// Support future tokenization of notes to anonymize a data model
    /// </summary>
    [JsonConverter(typeof(DaxNoteJsonConverter))]
    public class DaxNote
    {
        public string Note { get; }

        public DaxNote(string note)
        {
            this.Note = note;
        }

        private DaxNote() 
        {
        }

        public static bool operator ==(DaxNote a, DaxNote b)
        {
            return (a?.Note == b?.Note);
        }

        public static bool operator !=(DaxNote a, DaxNote b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.Note;
        }
    }
}
