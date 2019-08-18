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
    public class DaxExpression
    {
        public string Expression { get; private set; }
        private DaxExpression() { }
        private DaxExpression ( string expression )
        {
            this.Expression = expression;
        }
        static public DaxExpression GetExpression( string expression )
        {
            if (String.IsNullOrEmpty(expression)) {
                return null;
            }
            else {
                return new DaxExpression(expression);
            }
        }
    }
}
