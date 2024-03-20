using Dax.Metadata.JsonConverters;
using Newtonsoft.Json;

namespace Dax.Metadata
{
    /// <summary>
    /// Support future tokenization of expressions to anonymize a data model
    /// </summary>
    [JsonConverter(typeof(DaxExpressionJsonConverter))]
    public class DaxExpression
    {
        public string Expression { get; set; }

        private DaxExpression() { }

        public DaxExpression(string expression)
        {
            this.Expression = expression;
        }

        public static DaxExpression GetExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression)) {
                return null;
            }
            else {
                return new DaxExpression(expression);
            }
        }
    }
}
