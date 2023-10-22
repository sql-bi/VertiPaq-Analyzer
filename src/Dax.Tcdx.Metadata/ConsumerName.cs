using Dax.Metadata.JsonConverters;
using Newtonsoft.Json;
using Dax.Tcdx.Metadata.JsonConverters;

namespace Dax.Tcdx.Metadata
{
    /// <summary>
    /// Support future tokenization of names to anonymize a data model
    /// </summary>
    [JsonConverter(typeof(TcdxNameJsonConverter))]
    public class TcdxName

    {
        public string Name { get; }

        public TcdxName( string name )
        {
            this.Name = name;
        }

        private TcdxName()
        {
        }
       
        public static bool operator == ( TcdxName a, TcdxName b )
        {
            return (a?.Name == b?.Name);
        }

        public static bool operator !=(TcdxName a, TcdxName b)
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