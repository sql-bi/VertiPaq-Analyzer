using Dax.Metadata.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Metadata
{
    /// <summary>
    /// Support future tokenization of names to anonymize a data model
    /// </summary>
    [JsonConverter(typeof(DaxExpressionJsonConverter))]
    public class DaxExpression
    {
        public string Expression { get; private set; }
        private DaxExpression() { }
        public DaxExpression ( string expression )
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
