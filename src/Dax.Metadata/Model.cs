using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Dax.Metadata
{
    public class Model
    {
        public string DaxModelVersion { get; set; }
        /// <summary>
        /// Application that extracts the model info (e.g. DAX Studio, Tabular Editor, ...)
        /// </summary>
        public string ExtractorApp { get; set; }
        /// <summary>
        /// Version of application that extracts the model info
        /// </summary>
        public string ExtractorAppVersion { get; set; }
        /// <summary>
        /// Library that extracts the model info (e.g. Dax.Model.Extractor)
        /// </summary>
        public string ExtractorLib { get; set; }
        /// <summary>
        /// Version of the library that extracts the model info 
        /// </summary>
        public string ExtractorLibVersion { get; set; }
        /// <summary>
        /// Specifies settings used by the extractor
        /// </summary>
        public ExtractorProperties ExtractorProperties { get; set; }
        /// <summary>
        /// Library that manages the model info (e.g. Dax.Model)
        /// </summary>
        public string DaxModelLib { get; set; }
        /// <summary>
        /// LVersion of the library that manages the model info 
        /// </summary>
        public string DaxModelLibVersion { get; set; }
        public string ObfuscatorDictionaryId { get; set; }
        public string ObfuscatorLib { get; set; }
        public string ObfuscatorLibVersion { get; set; }

        public DaxName ServerName { get; set; }
        public DaxName ModelName { get; set; }

        /// <summary>
        /// Same compatibility level of TOM database
        /// </summary>
        public int CompatibilityLevel { get; set; }

        /// <summary>
        /// Same compatibility mode of TOM database
        /// </summary>
        public string CompatibilityMode { get; set; }

        /// <summary>
        /// Date/time in UTC of last extraction from DMV/TOM
        /// </summary>
        public DateTime ExtractionDate { get; set; }

        /// <summary>
        /// Date/time in UTC of the last refresh of the data model
        /// </summary>
        public DateTime LastDataRefresh { get; set; }

        /// <summary>
        /// Same last processed of TOM database
        /// </summary>
        public DateTime LastProcessed { get; set; }

        /// <summary>
        /// Same last update of TOM database
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Same version of TOM database
        /// </summary>
        public long Version { get; set; }

        public string ServerPaaSConnectionType { get; set; }
        public string ServerMode { get; set; }
        public string ServerLocation { get; set; }
        public string ServerVersion { get; set; }

        public List<Table> Tables { get; }
        public List<Relationship> Relationships { get; }
        public List<Role> Roles { get; }
        public List<Function> Functions { get; }

        /// <summary>
        /// Default partition mode
        /// </summary>
        public Partition.PartitionMode DefaultMode { get; set; }

        /// <summary>
        /// Model culture
        /// </summary>
        public string Culture { get; set; }

        public Model()
        {
            ExtractorProperties = new();
            this.Tables = new List<Table>();
            this.Relationships = new List<Relationship>();
            this.Roles = new List<Role>();
            this.Functions = new List<Function>();
        }

        // Manually update the version each time the DaxModel is modified - use https://semver.org/ specification
        [JsonIgnore]
        public static readonly string CurrentDaxModelVersion = new Version(1, 7, 0).ToString(3);

        public Model(string extractorLib, string extractorLibVersion, string extractorApp = null, string extractorAppVersion = null) : this()
        {
            this.ExtractorLib = extractorLib;
            this.ExtractorLibVersion = extractorLibVersion;
            this.ExtractorApp = extractorApp;
            this.ExtractorAppVersion = extractorAppVersion;

            var modelAssembly = this.GetType().Assembly;
            var modelAssemblyName = modelAssembly.GetName();
            var modelFileVersionInfo = FileVersionInfo.GetVersionInfo(modelAssembly.Location);

            this.DaxModelVersion = CurrentDaxModelVersion;
            this.DaxModelLib = modelAssemblyName.Name;
            this.DaxModelLibVersion = modelFileVersionInfo.ProductVersion; // e.g. CI build: 1.2.5-preview2+<git-commit-hash> , RELEASE build: 1.2.5
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            foreach (var f in Functions)
                f.Model = this;
        }

        /*
        public void PopulateColumnReferences()
        {
            var columns = from t in this.Tables
                          from c in t.Columns
                          select c;

            // Set IsReferenced of each column in the model
            // This initial scan set to true/false only
            foreach ( var c in columns )
            {
                c.IsReferenced = (FindReferences(c, true).Count > 0);
            }

            // Following scan set to TRUE columns that should be FALSE otherwise
            // We have to run these scans after the previous loop that resets the IsReferenced value

            // Relationship
            
            // Order by
        }

        public List<object> FindReferences( Column column, bool findFirstOnly )
        {
            var result = new List<object>();

            // We use the unqualified table name
            // A more reliable analysis requires a complete DAX parser
            string columnReferenceName = $"[{column.ColumnName.Name}]";

            // Scan columns 
            IEnumerable<object> columnExpressions =
                from t in this.Tables
                from c in t.Columns
                where (c.ColumnExpression?.Expression?.Contains(columnReferenceName)).GetValueOrDefault()
                select c;
            
            // Scan measures
            IEnumerable<object> measureExpressions = 
                from t in this.Tables
                from m in t.Measures
                where (m.MeasureExpression?.Expression?.Contains(columnReferenceName)).GetValueOrDefault()
                    || (m.DetailRowsExpression?.Expression?.Contains(columnReferenceName)).GetValueOrDefault()
                    || (m.KpiStatusExpression?.Expression?.Contains(columnReferenceName)).GetValueOrDefault()
                    || (m.KpiTargetExpression?.Expression?.Contains(columnReferenceName)).GetValueOrDefault()
                    || (m.KpiTrendExpression?.Expression?.Contains(columnReferenceName)).GetValueOrDefault()
                select m;

            // Scan calculated tables
            IEnumerable<object> tableExpressions = 
                from t in this.Tables
                where (t.TableExpression?.Expression?.Contains(columnReferenceName)).GetValueOrDefault()
                select t;

            // Scan calculation groups
            IEnumerable<object> calculationItemExpressions = 
                from t in this.Tables
                where t.CalculationGroup != null
                from ci in t.CalculationGroup.CalculationItems
                where (ci.ItemExpression?.Expression?.Contains(columnReferenceName)).GetValueOrDefault()
                    || (ci.FormatStringDefinition?.Expression?.Contains(columnReferenceName)).GetValueOrDefault()
                select ci;

            var completeList = columnExpressions.Union(measureExpressions).Union(tableExpressions).Union(calculationItemExpressions);

            foreach (var o in completeList)
            {
                if (findFirstOnly && result.Count > 0) break;
                result.Add(o);
            }

            return result;
        }
        */
    }

    public static class ModelExtensions
    {
        public static bool HasDirectLakePartitions(this Model model)
        {
            return model.Tables.Any((t) => t.HasDirectLakePartitions);
        }
    }
}
