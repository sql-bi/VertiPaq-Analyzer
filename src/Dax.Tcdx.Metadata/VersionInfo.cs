using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Tcdx.Metadata
{
    public class VersionInfo
    {

        // the TcdxVersion and TcdxLibVersion are defined by this project, so they are iniitialized here
        public VersionInfo()
        {
            this.TcdxVersion = "1.0.0.0";
            this.TcdxLibVersion = "1.0.0.0";
        }

        /// <summary>
        /// Version of the Tcdx model
        /// </summary>
        public string TcdxVersion { get; set; }
        /// <summary>
        /// Version of the library to anage the Tcdx model
        /// </summary>
        public string TcdxLibVersion { get; set; }
        /// <summary>
        /// Application that extracts the info (e.g. DAX Studio, Tabular Editor, ...)
        /// </summary>
        public string ExtractorApp { get; set; }
        /// <summary>
        /// Version of application that extracts the info
        /// </summary>
        public string ExtractorAppVersion { get; set; }
        /// <summary>
        /// Library that extracts the info 
        /// </summary>
        public string ExtractorLib { get; set; }
        /// <summary>
        /// Version of the library that extracts the info 
        /// </summary>
        public string ExtractorLibVersion { get; set; }
    }
}
