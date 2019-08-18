using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Model
{
    public class Model
    {
        public string DaxModelVersion { get; }
        /// <summary>
        /// Application that extracts the model info (e.g. DAX Studio, Tabular Editor, ...)
        /// </summary>
        public string ExtractorApp { get; }
        /// <summary>
        /// Version of application that extracts the model info
        /// </summary>
        public string ExtractorAppVersion { get; }
        /// <summary>
        /// Library that extracts the model info (e.g. Dax.Model.Extractor)
        /// </summary>
        public string ExtractorLib { get; }
        /// <summary>
        /// Version of the library that extracts the model info 
        /// </summary>
        public string ExtractorLibVersion { get; }
        /// <summary>
        /// Library that manages the model info (e.g. Dax.Model)
        /// </summary>
        public string DaxModelLib { get; }
        /// <summary>
        /// LVersion of the library that manages the model info 
        /// </summary>
        public string DaxModelLibVersion { get; }

        public string ModelName { get; set; }
        public int CompatibilityLevel { get; set; }

        public List<Table> Tables { get; }
        public List<Relationship> Relationships { get; }
        public List<Role> Roles { get; }
        public Model()
        {
            this.Tables = new List<Table>();
            this.Relationships = new List<Relationship>();
            this.Roles = new List<Role>();

            // TODO - how to support versioning?
            Version daxModelVersion = new Version(1, 0);
            this.DaxModelVersion = daxModelVersion.ToString();
            AssemblyName modelAssemblyName = this.GetType().Assembly.GetName();
            this.DaxModelLib = modelAssemblyName.Name;
            Version version = modelAssemblyName.Version;
            this.DaxModelLibVersion = version.ToString();
        }
        public Model(string extractorLib, string extractorLibVersion, string extractorApp = null, string extractorAppVersion = null) : this()
        {
            this.ExtractorLib = extractorLib;
            this.ExtractorLibVersion = extractorLibVersion;
            this.ExtractorApp = extractorApp;
            this.ExtractorAppVersion = extractorAppVersion;
        }
    }
}
