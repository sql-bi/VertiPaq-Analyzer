using Dax.Metadata.JsonConverters;
using Newtonsoft.Json;

namespace Dax.Metadata
{
    /// <summary>
    /// Support future tokenization of names to anonymize a data model
    /// </summary>
    [JsonConverter(typeof(DaxNameJsonConverter))]
    public class DaxName
    {
        public string Name { get; }

        public DaxName( string name )
        {
            this.Name = name;
        }

        private DaxName()
        {
        }
       
        public static bool operator == ( DaxName a, DaxName b )
        {
            return (a?.Name == b?.Name);
        }

        public static bool operator !=(DaxName a, DaxName b)
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
            return this.Name;
        }
    }
}