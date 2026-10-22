using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Vpax
{
    public static class VpaxFormat
    {
        public const string DAXMODEL = "DaxModel.json";
        public const string DAXVPAVIEW = "DaxVpaView.json";
        public const string TOMMODEL = "Model.bim";
        /// <summary>
        /// Name of the part with the compatibility mode of the database. The spelling "Compatability" is part of the format:
        /// existing files and readers use it, so it must not be corrected.
        /// </summary>
        public const string COMPATMODE = "CompatabilityMode.txt";

    }
}
