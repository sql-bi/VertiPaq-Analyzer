using Newtonsoft.Json;

namespace Dax.ViewVpaExport
{
    public class Function
    {
        [JsonIgnore]
        private readonly Dax.Metadata.Function _function;

        internal Function(Dax.Metadata.Function function)
        {
            _function = function;
        }

        public string Description => _function.Description?.Note;
        public string FunctionExpression => _function.FunctionExpression?.Expression;
        public string FunctionName => _function.FunctionName.Name;
        public bool IsHidden => _function.IsHidden;        
    }
}
