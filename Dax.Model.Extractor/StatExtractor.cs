using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using Microsoft.AnalysisServices.AdomdClient;

namespace Dax.Metadata.Extractor
{
    public class StatExtractor
    {
        protected Dax.Metadata.Model DaxModel { get; private set; }
        protected AdomdConnection Connection { get; private set; }
        protected int CommandTimeout { get; private set; } = 0;
        public StatExtractor (Dax.Metadata.Model daxModel, AdomdConnection connection )
        {
            this.DaxModel = daxModel;
            this.Connection = connection;
        }

        public static void UpdateStatisticsModel(Dax.Metadata.Model daxModel, AdomdConnection connection)
        {
            StatExtractor extractor = new StatExtractor(daxModel, connection);
            extractor.LoadTableStatistics();
            extractor.LoadColumnStatistics();

            // Update ExtractionDate
            extractor.DaxModel.ExtractionDate = DateTime.UtcNow;
        }

        public void LoadTableStatistics()
        {
            // only get table stats if the table has more than 1 user created column 
            // (every table has a RowNumber column so we only want tables with more than 1 column)
            var tableList = DaxModel.Tables.Where(t => t.Columns.Count > 1).Select(t => t.TableName.Name).ToList();
            var loopTables = tableList.SplitList(50);
            foreach ( var tableSet in loopTables ) {
                var dax = "EVALUATE ";
                //only union if there is more than 1 column in the columnSet
                if (tableSet.Count > 1) { dax += "UNION("; }
                dax += string.Join(",", tableSet.Select(tableName => $"\n    ROW(\"Table\", \"{EmbedNameInString(tableName)}\", \"Cardinality\", COUNTROWS('{tableName}'))").ToArray());
                //only close the union call if there is more than 1 column in the columnSet
                if (tableSet.Count > 1) { dax += ")"; }

                var cmd = new AdomdCommand(dax, Connection);
                cmd.CommandTimeout = CommandTimeout;
                using (var reader = cmd.ExecuteReader()) {

                    while (reader.Read()) {
                        var tableName = reader.GetString(0);
                        var cardinality = reader.IsDBNull(1) ? 0 : reader.GetInt64(1);

                        var table = DaxModel.Tables.Single(t => t.TableName.Name == tableName);
                        table.RowsCount = cardinality;
                    }
                }
            }
        }

        private static string Col(Column column)
        {
            return $"'{column.Table.TableName.Name}'[{column.ColumnName.Name.Replace("]", "]]")}]";
        }
        private static string EmbedNameInString(string originalName)
        {
            return originalName.Replace("\"", "\"\"");
        }
        public void LoadColumnStatistics()
        {
            var allColumns = (from t in DaxModel.Tables
                     from c in t.Columns
                     where c.State == "Ready"
                     select c).ToList();
            var loopColumns = allColumns.SplitList(50); // no more than 9999
            foreach ( var columnSet in loopColumns ) {
                var idString = 0;
                var dax = "EVALUATE ";
                //only union if there is more than 1 column in the columnSet
                if (columnSet.Count > 1) { dax += "UNION("; } 
                dax += string.Join(",", columnSet
                    .Where(c => !c.IsRowNumber )
                    .Select(c => $"\n    ROW(\"Table\", \"{idString++:0000}{EmbedNameInString(c.Table.TableName.Name)}\", \"Column\", \"{idString++:0000}{EmbedNameInString(c.ColumnName.Name)}\", \"Cardinality\", DISTINCTCOUNT({Col(c)}))").ToList());
                //only close the union call if there is more than 1 column in the columnSet
                if (columnSet.Count > 1) { dax += ")"; } 

                var cmd = new AdomdCommand(dax, Connection);
                cmd.CommandTimeout = CommandTimeout;
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        var tableName = reader.GetString(0).Substring(4);
                        var columnName = reader.GetString(1).Substring(4);
                        var cardinality = reader.IsDBNull(2) ? 0 : reader.GetInt64(2);

                        var column = DaxModel.Tables.Single(t => t.TableName.Name == tableName)
                                    .Columns.Single(c => c.ColumnName.Name == columnName);

                        column.ColumnCardinality = cardinality;
                    }
                }
            }
        }
    }
}
