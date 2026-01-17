using Dax.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.ViewVpaExport
{
    public class Function
    {
        private readonly Dax.Metadata.Function _function;
        internal Function(Dax.Metadata.Function function)
        {
            _function = function;
        }

        public string Name => _function.FunctionName.Name;
        public string Description => _function.Description?.Note;
        public bool IsHidden => _function.IsHidden;
        
    }
}
