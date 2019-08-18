using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Model
{
    /// <summary>
    /// Support future tokenization of names to anonymize a data model
    /// </summary>
    public class DaxName
    {
        public string Name { get; }

        public DaxName( string name )
        {
            this.Name = name;
        }
        private DaxName() { }
       
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
