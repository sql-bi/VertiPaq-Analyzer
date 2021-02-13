using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Metadata
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

        public DaxName ServerName { get; set; }
        public DaxName ModelName { get; set; }
        public int CompatibilityLevel { get; set; }

        /// <summary>
        /// Date/time in UTC of last extraction from DMV/TOM
        /// </summary>
        public DateTime ExtractionDate { get; set; }

        /// <summary>
        /// Date/time in UTC of the last refresh of the data model
        /// </summary>
        public DateTime LastDataRefresh { get; set; }

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
}
