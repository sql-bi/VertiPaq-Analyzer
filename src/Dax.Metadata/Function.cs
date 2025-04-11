using Newtonsoft.Json;
using System.Diagnostics;

namespace Dax.Metadata
{
    [DebuggerDisplay("{FunctionName.Name,nq}")]
    public class Function
    {
        public DaxNote Description { get; set; }

        public DaxExpression FunctionExpression { get; set; }

        public DaxName FunctionName { get; set; }

        public bool IsHidden { get; set; }

        [JsonIgnore]
        public Model Model { get; set; }
    }
}
