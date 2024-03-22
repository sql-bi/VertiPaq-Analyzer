using Dax.Metadata;
using Dax.Model.Extractor.Data;
using System;
using System.Data;
using System.Linq;

namespace Dax.Model.Extractor
{
    public class StatExtractor
    {
        protected Dax.Metadata.Model DaxModel { get; private set; }
        protected IDbConnection Connection { get; private set; }
        protected int CommandTimeout { get; private set; } = 0;
        public StatExtractor (Dax.Metadata.Model daxModel, IDbConnection connection )
        {
            this.DaxModel = daxModel;
            this.Connection = connection;
        }

        protected IDbCommand CreateCommand(string commandText)
        {
            return Connection.CreateCommand(commandText);
        }

        public static void UpdateStatisticsModel(Dax.Metadata.Model daxModel, IDbConnection connection, int sampleRows = 0, bool analyzeDirectQuery = false , DirectLakeExtractionMode analyzeDirectLake = DirectLakeExtractionMode.ResidentOnly)
        {
            StatExtractor extractor = new StatExtractor(daxModel, connection);
            extractor.LoadTableStatistics(analyzeDirectQuery, analyzeDirectLake);
            extractor.LoadColumnStatistics(analyzeDirectQuery, analyzeDirectLake);
            extractor.LoadRelationshipStatistics(sampleRows, analyzeDirectQuery, analyzeDirectLake);

            // Update ExtractionDate
            extractor.DaxModel.ExtractionDate = DateTime.UtcNow;
        }

        public void LoadRelationshipStatistics(int sampleRows = 0,bool analyzeDirectQuery = false, DirectLakeExtractionMode analyzeDirectLake = DirectLakeExtractionMode.ResidentOnly)
        {
            // Maximum number of invalid keys used for extraction through SAMPLE, use TOPNSKIP or TOPN otherwise
            const int MAX_KEYS_FOR_SAMPLE = 1000;

            // only get relationship stats if the relationship has an ID
            var relationshipList = DaxModel.Relationships.Where(r => r.Dmv1200RelationshipId != 0)
                .Where(rel =>
                    // we are analyzing DQ and either column is in a table with DQ partitions
                    (analyzeDirectQuery && ((rel.FromColumn.Table.HasDirectQueryPartitions) || (rel.ToColumn.Table.HasDirectQueryPartitions)))

                    // or we are analyzing DL and either column is in a table with DL partitions
                    || (analyzeDirectLake >= DirectLakeExtractionMode.Referenced && ((rel.FromColumn.Table.HasDirectLakePartitions) || (rel.ToColumn.Table.HasDirectLakePartitions))

                    // or if both columns are resident in memory
                    || (rel.FromColumn.IsResident && rel.ToColumn.IsResident)

                    // or neither table has DQ or DL partitions
                    || ((!rel.FromColumn.Table.HasDirectQueryPartitions) && (!rel.ToColumn.Table.HasDirectQueryPartitions) &&
                        (!rel.FromColumn.Table.HasDirectLakePartitions) && (!rel.ToColumn.Table.HasDirectLakePartitions))
                    )
                ).ToList();

            var loopRelationships = relationshipList.SplitList(10);
            #region Statistics
            foreach (var relationshipSet in loopRelationships)
            {
                // Skip EVALUATE if no valid relationships are found
                if (!relationshipSet.Any()) continue;

                var daxStatistics = "EVALUATE ";
                //only union if there is more than 1 column in the columnSet
                if (relationshipSet.Count > 1) { daxStatistics += "UNION("; }
                daxStatistics += string.Join(
                    ",",
                    relationshipSet
                        .Select(rel => $@"
CALCULATETABLE (
ROW( 
    ""RelationshipId"", {rel.Dmv1200RelationshipId},
    ""MissingKeys"", DISTINCTCOUNT ( {EscapeColumnName(rel.FromColumn)} ),
    ""InvalidRows"", COUNTROWS({EscapeTableName(rel.FromColumn.Table)})
),
ISBLANK( {EscapeColumnName(rel.ToColumn)} ),
USERELATIONSHIP( {EscapeColumnName(rel.FromColumn)}, {EscapeColumnName(rel.ToColumn)} )
)").ToArray());
                //             ""Column"", ""{EmbedNameInString(rel.FromColumn.ColumnName.Name)}"", 

                //only close the union call if there is more than 1 column in the columnSet
                if (relationshipSet.Count > 1) { daxStatistics += ")"; }

                var cmdStatistics = CreateCommand(daxStatistics);
                cmdStatistics.CommandTimeout = CommandTimeout;
                    
                using (var reader = cmdStatistics.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        var relationshipId = reader.IsDBNull(0) ? 0 : reader.GetInt64(0);
                        var missingKeys = reader.IsDBNull(1) ? 0 : reader.GetInt64(1);
                        var invalidRows = reader.IsDBNull(2) ? 0 : reader.GetInt64(2);

                        var relationship = DaxModel.Relationships.Single(r => r.Dmv1200RelationshipId == relationshipId);
                        relationship.MissingKeys = missingKeys;
                        relationship.InvalidRows = invalidRows;
                    }
                }
            }
            #endregion
            if (sampleRows > 0)
            {
                var loopInvalidRelationships = relationshipList.Where(r => r.InvalidRows > 0).ToList().SplitList(10);
                #region Details
                foreach (var relationshipSet in loopInvalidRelationships)
                {
                    // Skip EVALUATE if no valid relationships are found
                    if (!relationshipSet.Any()) continue;

                    var daxDetails = "EVALUATE ";
                    //only union if there is more than 1 column in the columnSet
                    if (relationshipSet.Count > 1) { daxDetails += "UNION("; }
                    daxDetails += string.Join(
                        ",",
                        relationshipSet.Select(rel => (rel.MissingKeys > sampleRows && rel.MissingKeys <= MAX_KEYS_FOR_SAMPLE) ?
$@"CALCULATETABLE ( 
SELECTCOLUMNS ( 
    SAMPLE ( {sampleRows}, DISTINCT ( {EscapeColumnName(rel.FromColumn)} ), {EscapeColumnName(rel.FromColumn)}, ASC ), 
    ""RelationshipId"", {rel.Dmv1200RelationshipId}, 
    ""MissingValue"", {EscapeColumnName(rel.FromColumn)} 
),
ISBLANK( {EscapeColumnName(rel.ToColumn)} ),
USERELATIONSHIP( {EscapeColumnName(rel.FromColumn)}, {EscapeColumnName(rel.ToColumn)} )
)"
: (DaxModel.CompatibilityLevel >= 1200) ?
// Use TOPNSKIP to get 10*sampleRows and then use TOPN/DISTINCT to not include too many sample
// This is required because TOPNSKIP could have duplicated values.
// TOPNSKIP is required for large cardinality columns to avoid out-of-memory errors
$@"CALCULATETABLE ( 
    TOPN ( {sampleRows}, DISTINCT ( SELECTCOLUMNS (
        TOPNSKIP ( {sampleRows*10}, 0, {EscapeTableName(rel.FromColumn.Table)} ),
    ""RelationshipId"", {rel.Dmv1200RelationshipId}, 
    ""MissingValue"", {EscapeColumnName(rel.FromColumn)} 
))),
ISBLANK( {EscapeColumnName(rel.ToColumn)} ),
USERELATIONSHIP( {EscapeColumnName(rel.FromColumn)}, {EscapeColumnName(rel.ToColumn)} )
)"
:
// Use TOPN with SSAS 2012/2014 and PowerPivot
$@"CALCULATETABLE ( 
SELECTCOLUMNS ( 
    TOPN ( {sampleRows}, DISTINCT ( {EscapeColumnName(rel.FromColumn)} ) ), 
    ""RelationshipId"", {rel.Dmv1200RelationshipId}, 
    ""MissingValue"", {EscapeColumnName(rel.FromColumn)} 
),
ISBLANK( {EscapeColumnName(rel.ToColumn)} ),
USERELATIONSHIP( {EscapeColumnName(rel.FromColumn)}, {EscapeColumnName(rel.ToColumn)} )
)"

).ToArray());


                    //only close the union call if there is more than 1 column in the columnSet
                    if (relationshipSet.Count > 1) { daxDetails += ")"; }

                    var cmdDetails = CreateCommand(daxDetails);
                    cmdDetails.CommandTimeout = CommandTimeout;

                    using (var reader = cmdDetails.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var relationshipId = reader.IsDBNull(0) ? 0 : reader.GetInt64(0);
                            var missingValue = reader.IsDBNull(1) ? "(blank)" : reader.GetValue(1).ToString();

                            var relationship = DaxModel.Relationships.Single(r => r.Dmv1200RelationshipId == relationshipId);
                            relationship.SampleReferentialIntegrityViolations.Add(missingValue);
                        }
                    }
                }
            }
            #endregion
        }

        public void LoadTableStatistics( bool analyzeDirectQuery = false , DirectLakeExtractionMode analyzeDirectLake = DirectLakeExtractionMode.ResidentOnly)
        {
            // only get table stats if the table has more than 1 user created column 
            // (every table has a RowNumber column so we only want tables with more than 1 column)
            var tableList = DaxModel.Tables
                .Where( t => t.Columns.Count > 1 && (analyzeDirectQuery || !t.HasDirectQueryPartitions) && (analyzeDirectLake >= DirectLakeExtractionMode.ResidentOnly || !t.HasDirectLakePartitions) )
                .Select(t => t).ToList();
            var loopTables = tableList.SplitList(50);
            foreach ( var tableSet in loopTables ) {
                var dax = "EVALUATE ";
                //only union if there is more than 1 column in the columnSet
                if (tableSet.Count > 1) { dax += "UNION("; }
                dax += string.Join(
                    ",", 
                    tableSet.Select(table => $"\n    ROW(\"Table\", \"{EmbedNameInString(table.TableName.Name)}\", \"Cardinality\", COUNTROWS({EscapeTableName(table)}))").ToArray());
                //only close the union call if there is more than 1 column in the columnSet
                if (tableSet.Count > 1) { dax += ")"; }

                var cmd = CreateCommand(dax);
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

        private static string EscapeTableName(Table table)
        {
            return $"'{table.TableName.Name.Replace("'", "''")}'";
        }

        private static string DistinctCountExpression(Column column)
        {
            return column.GroupByColumns.Count == 0 
                ? $"DISTINCTCOUNT({EscapeColumnName(column)})"
                : $"COUNTROWS(ALLNOBLANKROW({EscapeColumnName(column)}))";
        }

        private static string EscapeColumnName(Column column)
        {
            return $"{EscapeTableName(column.Table)}[{column.ColumnName.Name.Replace("]", "]]")}]";
        }
        private static string EmbedNameInString(string originalName)
        {
            return originalName.Replace("\"", "\"\"");
        }
        public void LoadColumnStatistics(bool analyzeDirectQuery = false, DirectLakeExtractionMode analyzeDirectLake = DirectLakeExtractionMode.ResidentOnly)
        {
            var allColumns = 
                (from t in DaxModel.Tables
                 // skip direct query tables if the analyzeDirectQuery is false
                 where t.Columns.Count > 1 && (analyzeDirectQuery || !t.HasDirectQueryPartitions)   
                     from c in t.Columns
                     where c.State == "Ready"
                        // only include the column if the table does not have Direct Lake partitions or if they are resident or if analyzeDirectLake is true
                        && (!t.HasDirectLakePartitions 
                            || (analyzeDirectLake >= DirectLakeExtractionMode.ResidentOnly && c.IsResident) 
                            || (analyzeDirectLake >= DirectLakeExtractionMode.Referenced && c.IsReferenced )
                            || (analyzeDirectLake == DirectLakeExtractionMode.Full)
                            )
                     select c).ToList();
            var loopColumns = allColumns.SplitList(50); // no more than 9999
            foreach ( var columnSet in loopColumns ) {
                var idString = 0;
                var dax = "EVALUATE ";
                int validColumns = columnSet.Where(c => !c.IsRowNumber).Count();
                //only union if there is more than 1 column in the columnSet
                if (validColumns > 1) { dax += "UNION("; } 
                dax += string.Join(",", columnSet
                    .Where(c => !c.IsRowNumber && c.GroupByColumns.Count == 0 )
                    .Select(c => $"\n    ROW(\"Table\", \"{idString++:0000}{EmbedNameInString(c.Table.TableName.Name)}\", \"Column\", \"{idString++:0000}{EmbedNameInString(c.ColumnName.Name)}\", \"Cardinality\", {DistinctCountExpression(c)})").ToList());
                //only close the union call if there is more than 1 column in the columnSet
                if (validColumns > 1) { dax += ")"; }

                var cmd = CreateCommand(dax);
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
