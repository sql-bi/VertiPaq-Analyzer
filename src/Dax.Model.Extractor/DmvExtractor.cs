using Dax.Model.Extractor.Data;
using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Tom = Microsoft.AnalysisServices.Tabular;

namespace Dax.Metadata.Extractor
{

    public class ExtractorException : Exception
    {
        public IDbConnection Connection { get; private set; }
        public string DatabaseName { get; private set; }
        public ExtractorException( IDbConnection connection, string databaseName )
        {
            Connection = connection;
            DatabaseName = databaseName;
        }
        public ExtractorException(IDbConnection connection)
        {
            Connection = connection;
        }
    }
    public class DmvExtractor
    {
        public int CommandTimeout { get; set; } = 0;

        protected IDbConnection Connection { get; private set; }

        public Dax.Metadata.Model DaxModel { get; private set; }

        protected IDbCommand CreateCommand(string commandText)
        {
            return Connection.CreateCommand(commandText);
        }

        public DmvExtractor(Dax.Metadata.Model daxModel, IDbConnection connection, string serverName, string databaseName, string extractorApp, string extractorVersion)
        {
            Connection = connection;
            Connection.Open();

            // Validate databaseName
            var compatibilityLevel = daxModel.CompatibilityLevel; // assume the default obtained by TOM if any, or unknown/0 otherwise
            if (!CheckDatabaseNameCompatibilityLevel(ref databaseName, ref compatibilityLevel)) {
                throw new ExtractorException(connection, databaseName);
            }

            var extractorInfo = Util.GetExtractorInfo(this);
            // Create a DAX model if it is not provided in the constructor arguments
            // This should be outsider the constructor, but we want to make sure the database name is 
            // validated before using other DMVs
            DaxModel = daxModel ?? new Dax.Metadata.Model(extractorInfo.Name, extractorInfo.Version, extractorApp, extractorVersion);
            DaxModel.ServerName = new DaxName(serverName);
            DaxModel.ModelName = new DaxName(databaseName);
            DaxModel.CompatibilityLevel = compatibilityLevel;

            // Update ExtractionDate
            DaxModel.ExtractionDate = DateTime.UtcNow;
        }


        public static void PopulateFromDmv(Dax.Metadata.Model daxModel, IDbConnection connection, string serverName, string databaseName, string extractorApp, string extractorVersion)
        {
            Dax.Metadata.Extractor.DmvExtractor de = new Dax.Metadata.Extractor.DmvExtractor(daxModel, connection, serverName, databaseName, extractorApp, extractorVersion);
            de.PopulateModel();
            de.PopulateTables();
            de.PopulatePartitions();
            de.PopulateColumns();
            de.PopulateMeasures();
            de.PopulateLastDataUpdate();
            de.PopulateUserHierarchies();
            de.PopulateRelationships();
            de.PopulateReferences();
        }

        protected bool CheckDatabaseNameCompatibilityLevel(ref string databaseName, ref int compatibilityLevel)
        {
            const string QUERY_CATALOGS = @"
SELECT [CATALOG_NAME]
FROM $SYSTEM.DBSCHEMA_CATALOGS";

            var cmd = CreateCommand(QUERY_CATALOGS);
            cmd.CommandTimeout = CommandTimeout;
            var catalogName = string.Empty;
            
            using (var rdr = cmd.ExecuteReader()) {

                while (rdr.Read()) {
                    catalogName = rdr.GetString(0);
                    
                    if (catalogName.Equals(databaseName, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }

            // need to get the compatibility level outside of the loop to check the database name
            // so that we are not trying to run 2 queries at the same time on the same connection
            if (catalogName.Equals(databaseName, StringComparison.OrdinalIgnoreCase))
            {
                // Copy validated catalog name in databasename to avoid injection.
                databaseName = catalogName;
                compatibilityLevel = GetCompatibilityLevel(databaseName);
                return true;
            }

            // no matches found
            return false;
        }

        protected int GetCompatibilityLevel(string databaseName)
        {
            string QUERY_CATALOGS = $@"
SELECT [COMPATIBILITY_LEVEL]
FROM $SYSTEM.DBSCHEMA_CATALOGS
WHERE [CATALOG_NAME] = '{databaseName}'";

            var cmd = CreateCommand(QUERY_CATALOGS);
            cmd.CommandTimeout = CommandTimeout;
            int compatibilityLevel = 0;

            try
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        compatibilityLevel = rdr.GetInt32(0);
                        return compatibilityLevel;
                    }
                }
            }
            catch
            {
                // if there was an error getting the COMPATIBILITY_LEVEL column
                // this is probably an old version of the engine
                compatibilityLevel = 0;
            }

            
            // no matches found
            return compatibilityLevel;
        }

        /// <summary>
        // Search a DAX Column or create one if it does not exists
        // It also creates the table if it does not exists
        /// </summary>
        /// <param name="tableName">Name of the table to search</param>
        /// <param name="columnId">Id name (in DMV 1100) of the column to search</param>
        /// <returns></returns>
        private Column GetDaxColumnDmv1100Id(string tableName, string columnDmv1100Id)
        {
            var daxTable = GetDaxTable(tableName);
            var daxColumn = daxTable.Columns.FirstOrDefault(t => t.Dmv1100ColumnId?.Equals(columnDmv1100Id) == true);
            if (daxColumn == null) {
                daxColumn = new Dax.Metadata.Column(daxTable);
                daxColumn.SetDmv1100ColumnId(columnDmv1100Id);

                daxTable.Columns.Add(daxColumn);
            }

            return daxColumn;
        }

        /// <summary>
        /// Search an existing column in DMV 1200
        /// </summary>
        /// <param name="tableDmv1200Id">ID in DMV 1200 of the table to search</param>
        /// <param name="columnDmv1200Id">ID in DMV 1200 of the column to search</param>
        /// <returns>Returns null if the column is not found</returns>
        private Column GetDaxColumnDmv1200Id(int tableDmv1200Id, int columnDmv1200Id)
        {
            var daxTable = GetDaxTableDmv1200Id(tableDmv1200Id);
            var daxColumn = daxTable?.Columns.FirstOrDefault(t => t.Dmv1200ColumnId == columnDmv1200Id);
            return daxColumn;
        }


        /// <summary>
        // Search a DAX Table or create one if it does not exist
        /// </summary>
        /// <param name="tableName">Name of the table to search</param>
        /// <returns></returns>
        private Table GetDaxTable(string tableName)
        {
            var daxTable = DaxModel.Tables.FirstOrDefault(t => t.TableName.Name.Equals(tableName));
            if (daxTable == null) {
                daxTable = new Dax.Metadata.Table(DaxModel)
                {
                    TableName = new Dax.Metadata.DaxName(tableName)
                };

                DaxModel.Tables.Add(daxTable);
            }

            return daxTable;
        }

        /// <summary>
        // Search a DAX Table only if exists
        /// </summary>
        /// <param name="tableName">ID in DMV 1200 of the table to search</param>
        /// <returns>Returns null if the table does not exists</returns>
        private Table GetDaxTableDmv1200Id(int tableDmv1200Id)
        {
            var daxTable = DaxModel.Tables.FirstOrDefault(t => t.Dmv1200TableId == tableDmv1200Id);
            return daxTable;
        }

        /// <summary>
        // Search a DAX Column or create one if it does not exists
        // It also creates the table if it does not exists
        /// </summary>
        /// <param name="tableName">Name of the table to search</param>
        /// <param name="columnName">Name of the column to search</param>
        /// <returns></returns>
        private Column GetDaxColumn(string tableName, string columnName)
        {
            var daxTable = GetDaxTable(tableName);
            var daxColumn = daxTable.Columns.FirstOrDefault(t => t.ColumnName.Name.Equals(columnName));
            if (daxColumn == null) {
                daxColumn = new Dax.Metadata.Column(daxTable)
                {
                    ColumnName = new Dax.Metadata.DaxName(columnName),
                    // Using the RowNumber name is not a safe technique, but it should work most of the times for DMV 1100
                    IsRowNumber = (columnName == "RowNumber")  
                                  || columnName.StartsWith("RowNumber-")
                };

                daxTable.Columns.Add(daxColumn);
            }

            return daxColumn;
        }

        private Partition GetDaxPartition(string tableName, string partitionName)
        {
            var daxTable = GetDaxTable(tableName);
            var daxPartition = daxTable.Partitions.FirstOrDefault(p => p.PartitionName.Name.Equals(partitionName));
            if (daxPartition == null) {
                daxPartition = new Dax.Metadata.Partition(daxTable)
                {
                    PartitionName = new Dax.Metadata.DaxName(partitionName)
                };

                daxTable.Partitions.Add(daxPartition);
            }

            return daxPartition;
        }

        private ColumnSegment GetDaxColumnSegment(string tableName, string partitionName, string columnDmv1100Id, long segmentNumber, long tablePartitionNumber)
        {
            var daxColumn = GetDaxColumnDmv1100Id(tableName, columnDmv1100Id);
            var daxPartition = GetDaxPartition(tableName, partitionName);
            daxPartition.PartitionNumber = tablePartitionNumber;
            var daxColumnSegment = daxColumn.ColumnSegments.FirstOrDefault(s => s.SegmentNumber == segmentNumber);
            if (daxColumnSegment == null) {
                daxColumnSegment = new Dax.Metadata.ColumnSegment(daxColumn, daxPartition)
                {
                    SegmentNumber = segmentNumber
                };

                daxColumn.ColumnSegments.Add(daxColumnSegment);
            }

            return daxColumnSegment;
        }

        private ColumnHierarchy GetDaxColumnHierarchyDmv1100Id(string tableName, string columnDmv1100Id, string structureName, long tablePartitionNumber, long segmentNumber)
        {
            var daxColumn = GetDaxColumnDmv1100Id(tableName, columnDmv1100Id);
            var daxColumnHierarchy =
                daxColumn.ColumnHierarchies.FirstOrDefault(
                    h =>
                        h.StructureName.Name == structureName
                        && h.TablePartitionNumber == tablePartitionNumber
                        && h.SegmentNumber == segmentNumber
                );
            if (daxColumnHierarchy == null) {
                daxColumnHierarchy = new Dax.Metadata.ColumnHierarchy(daxColumn)
                {
                    StructureName = new Dax.Metadata.DaxName(structureName),
                    TablePartitionNumber = tablePartitionNumber,
                    SegmentNumber = segmentNumber
                };

                daxColumn.ColumnHierarchies.Add(daxColumnHierarchy);
            }

            return daxColumnHierarchy;
        }


        private UserHierarchy GetDaxUserHierarchy(string tableName, string userHierarchyName)
        {
            var daxTable = GetDaxTable(tableName);
            var daxUserHierarchy =
                daxTable.UserHierarchies.FirstOrDefault(h => h.HierarchyName.Name == userHierarchyName);
            if (daxUserHierarchy == null) {
                daxUserHierarchy = new Dax.Metadata.UserHierarchy(daxTable)
                {
                    HierarchyName = new Dax.Metadata.DaxName(userHierarchyName)
                };
                daxTable.UserHierarchies.Add ( daxUserHierarchy );
            }

            return daxUserHierarchy;
        }


        public void PopulateTables()
        {
            const string QUERY_TABLES = @"
SELECT 
    DIMENSION_NAME AS TABLE_NAME, 
    TABLE_ID       AS TABLE_ID,
    ROWS_COUNT     AS ROWS_IN_TABLE,
    RIVIOLATION_COUNT AS RI_VIOLATION_COUNT
FROM  $SYSTEM.DISCOVER_STORAGE_TABLES
WHERE RIGHT ( LEFT ( TABLE_ID, 2 ), 1 ) <> '$'
ORDER BY DIMENSION_NAME";

            var cmd = CreateCommand(QUERY_TABLES);
            cmd.CommandTimeout = CommandTimeout;

            using (var rdr = cmd.ExecuteReader()) {
                while (rdr.Read()) {
                    string tableName = rdr.GetString(0);
                    string tableId = rdr.GetString(1);
                    long rowsCount = rdr.GetInt64(2);
                    long referentialIntegrityViolationCount = rdr.GetInt64(3);

                    Table daxTable = GetDaxTable(tableName);
                    daxTable.SetDmv1100TableId(tableId);
                    daxTable.RowsCount = rowsCount;
                    daxTable.ReferentialIntegrityViolationCount = referentialIntegrityViolationCount;
                }
            }
        }

        public void PopulateMeasures()
        {
            const string QUERY_KPIS = @"
SELECT 
    MEASUREGROUP_NAME AS TABLE_NAME,
    KPI_NAME,
    KPI_VALUE AS KPI_MEASURE_UNIQUE_NAME,
    KPI_GOAL AS KPI_GOAL_UNIQUE_NAME,
    KPI_STATUS AS KPI_STATUS_UNIQUE_NAME,
    KPI_TREND AS KPI_TREND_UNIQUE_NAME
FROM $SYSTEM.MDSCHEMA_KPIS
ORDER BY MEASUREGROUP_NAME";
            const string QUERY_MEASURES = @"
SELECT 
    MEASUREGROUP_NAME AS TABLE_NAME,
    MEASURE_NAME,
    DATA_TYPE,
    EXPRESSION,
    DEFAULT_FORMAT_STRING,
    MEASURE_IS_VISIBLE,
    MEASURE_DISPLAY_FOLDER,
    MEASURE_UNIQUE_NAME,
    [DESCRIPTION]
FROM $SYSTEM.MDSCHEMA_MEASURES
WHERE MEASURE_NAME <> '__Default measure'
ORDER BY MEASUREGROUP_NAME";
            const string QUERY_FORMATSTRINGS = @"
SELECT 
    [ID] AS FORMAT_ID,
    [Expression] AS FORMAT_STRING_EXPRESSION
FROM $SYSTEM.TMSCHEMA_FORMAT_STRING_DEFINITIONS
";
            const string QUERY_MEASURESFORMATID = @"
SELECT 
    [FormatStringDefinitionID] AS FORMAT_ID,
    [Name] AS MEASURE_NAME
FROM $SYSTEM.TMSCHEMA_MEASURES
WHERE [FormatStringDefinitionID] > 0

";

            Dictionary<string, string> formatStringExpressions = new Dictionary<string, string>();
            List<string> kpiInternalMeasures = new List<string>();
            ReadKpis();
            ReadFormatStringExpressions();
            ProcessMeasures();

            void ReadKpis()
            {
                var cmd = CreateCommand(QUERY_KPIS);
                cmd.CommandTimeout = CommandTimeout;

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string tableName = rdr.GetString(0) ?? string.Empty;
                        string kpiName = rdr.GetString(1) ?? string.Empty;
                        string kpiValue = rdr.GetString(2) ?? string.Empty;
                        string kpiGoal = rdr.GetString(3) ?? string.Empty;
                        string kpiStatus = rdr.GetString(4) ?? string.Empty;
                        string kpiTrend = rdr.GetString(5) ?? string.Empty;

                        if (!string.IsNullOrEmpty(kpiGoal)) kpiInternalMeasures.Add(kpiGoal);
                        if (!string.IsNullOrEmpty(kpiStatus)) kpiInternalMeasures.Add(kpiStatus);
                        if (!string.IsNullOrEmpty(kpiTrend)) kpiInternalMeasures.Add(kpiTrend);
                    }
                }
            }

            void ReadFormatStringExpressions()
            {
                Dictionary<long,string> mapExpressions = new Dictionary<long, string>();

                // read all format string expressions into a dictionary
                using var cmd = CreateCommand(QUERY_FORMATSTRINGS);
                cmd.CommandTimeout = CommandTimeout;
                try {
                    using (var rdr = cmd.ExecuteReader()) {
                        while (rdr.Read()) {
                            long formatId = rdr.GetInt64(0);
                            string formatStringExpression = rdr.GetValue(1)?.ToString() ?? string.Empty;
                            if (formatId > 0 && !string.IsNullOrEmpty(formatStringExpression)) {
                                mapExpressions.Add(formatId, formatStringExpression);
                            }
                        }
                    }
                }
                catch (Exception) { 
                    // we will get an error if the current server does not support dynamic format strings
                    // we can just swallow this and go on as if no dynamic format strings are defined
                }
                // if there are no format string expressions exit here
                if (mapExpressions.Count == 0) return;

                try {
                    // map the format string expressions to the measure names
                    using (var cmdId = CreateCommand(QUERY_MEASURESFORMATID)) {
                        cmdId.CommandTimeout = CommandTimeout;
                        using (var rdr = cmdId.ExecuteReader()) {
                            while (rdr.Read()) {
                                long formatId = rdr.GetInt64(0);
                                string measureName = rdr.GetValue(1)?.ToString() ?? string.Empty;
                                if (formatId > 0 && !string.IsNullOrEmpty(measureName)) {
                                
                                    if (mapExpressions.ContainsKey(formatId)) {
                                        formatStringExpressions.Add(measureName, mapExpressions[formatId]);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // we will get an error if the current model has no dynamic format strings
                    // we can just swallow this and go on as if no dynamic format strings are defined
                }
            }

            void ProcessMeasures()
            {
                var cmd = CreateCommand(QUERY_MEASURES);
                cmd.CommandTimeout = CommandTimeout;

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string tableName = (rdr.GetValue(0) as string) ?? string.Empty;
                        string measureName = rdr.GetString(1);
                        int dataType = rdr.GetInt32(2);
                        string formatStringExpression = formatStringExpressions.ContainsKey(measureName) ? formatStringExpressions[measureName] : string.Empty;
                        string measureExpression = rdr.GetValue(3)?.ToString() ?? string.Empty;
                        string defaultFormatString = rdr.GetValue(4)?.ToString() ?? string.Empty;
                        bool measureVisible = rdr.GetBoolean(5);
                        string measureDisplayFolder = rdr.GetString(6);
                        string measureUniqueName = rdr.GetString(7) ?? string.Empty;
                        string measureDescription = rdr.GetString(8);

                        Table daxTable = GetDaxTable(tableName);
                        var daxMeasure = daxTable.Measures.FirstOrDefault(m => m.MeasureName.Name == measureName);
                        if (daxMeasure == null && 
                                // Does not add hidden measures created for KPIs
                                (string.IsNullOrEmpty(measureUniqueName) || !kpiInternalMeasures.Contains(measureUniqueName)) )
                        {
                            daxMeasure = new Dax.Metadata.Measure()
                            {
                                Table = daxTable,
                                MeasureName = new DaxName(measureName),
                                // dataType not set?
                                MeasureExpression = new DaxExpression(measureExpression),
                                FormatString = defaultFormatString, 
                                FormatStringExpression = new DaxExpression(formatStringExpression),
                                IsHidden = !measureVisible,
                                DisplayFolder = new DaxNote(measureDisplayFolder), 
                                Description = new DaxNote(measureDescription)
                            };

                            daxTable.Measures.Add(daxMeasure);
                        }
                    }
                }
            }
        }

        private Dictionary<long, string> GetTableIds()
        {
            const string QUERY_TABLE_IDS = @"
SELECT[ID], [Name]
FROM $SYSTEM.TMSCHEMA_TABLES";

            var map = new Dictionary<long, string>();

            // Skip the PopulateRelationships task if the compatibility level is older than TOM
            if (DaxModel.CompatibilityLevel >= 1200)
            {
                var cmd = CreateCommand(QUERY_TABLE_IDS);
                cmd.CommandTimeout = CommandTimeout;

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        long tableId = (long)rdr.GetDecimal(0);
                        string tableName = rdr.GetString(1);
                        map.Add(tableId, tableName);
                    }
                }
            }

            return map;
        }

        public void PopulateModel()
        {
            // Skip the PopulateRelationships task if the compatibility level is older than TOM
            if (this.DaxModel.CompatibilityLevel < 1200) {
                return;
            }
            const string QUERY_MODELS = @"
SELECT
    [ID] AS MODEL_ID,
    [Name] AS MODEL_NAME,
    [DefaultMode] AS PARTITION_DEFAULT_MODE,
    [DefaultDataView] AS DEFAULT_DATA_VIEW,
    [Culture] AS MODEL_CULTURE
FROM $SYSTEM.TMSCHEMA_MODEL
ORDER BY [ID]";
            // Note 2022-08-24
            // We removed the field ForceUniqueNames as it is not supported in compatibility levels
            // earlier than 1500 (to be verified). In case we want to restore it, add the following
            // column to the previous query:
            // ,[ForceUniqueNames] AS MODEL_FORCE_UNIQUE_NAMES


            var cmd = CreateCommand(QUERY_MODELS);
            cmd.CommandTimeout = CommandTimeout;

            using (var rdr = cmd.ExecuteReader()) {
                // Read only one row - we do not expect multiple models in a single database
                if (rdr.Read()) {
                    // long modelId = (long)rdr.GetDecimal(0);
                    // string modelName = rdr.GetString(1);
                    long defaultMode = rdr.GetInt64(2);
                    // long defaultDataView = rdr.GetInt64(3);
                    string modelCulture = rdr.GetString(4);
                    // bool forceUniqueNames = rdr.GetBoolean(5);

                    this.DaxModel.DefaultMode = (Partition.PartitionMode)defaultMode;
                    this.DaxModel.Culture = modelCulture;
                }
            }
        }

        public void PopulatePartitions()
        {
            // Skip the PopulateRelationships task if the compatibility level is older than TOM
            if (this.DaxModel.CompatibilityLevel < 1200)
            {
                return;
            }

            const string QUERY_PARTITIONS = @"
SELECT 
    [ID] AS PARTITION_ID,
    [TableID] AS TABLE_ID,
    [Name] AS PARTITION_NAME,
    [Description] AS PARTITION_DESCRIPTION,
    [State] AS PARTITION_STATE,
    [Type] AS PARTITION_TYPE,
    [Mode] AS PARTITION_MODE,
    [RefreshedTime] AS REFRESHED_TIME
FROM $SYSTEM.TMSCHEMA_PARTITIONS
ORDER BY [TableID]";

            var mapTableIds = GetTableIds();

            var cmd = CreateCommand(QUERY_PARTITIONS);
            cmd.CommandTimeout = CommandTimeout;

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    // long partitionId = (long)rdr.GetDecimal(0);
                    long tableId = (long)rdr.GetDecimal(1);
                    string partitionName = rdr.GetString(2);
                    string description = !rdr.IsDBNull(3) ? rdr.GetString(3) : null;
                    long state = rdr.GetInt64(4);
                    long type = rdr.GetInt64(5);
                    long mode = rdr.GetInt64(6);
                    DateTime? refreshedTime = !rdr.IsDBNull(7) ? rdr.GetDateTime(7) : (DateTime?) null;

                    if (mapTableIds.TryGetValue(tableId, out string tableName))
                    {
                        // Table daxTable = GetDaxTable(tableName);
                        var daxPartition = GetDaxPartition(tableName, partitionName);
                        daxPartition.Description = new DaxNote(description);
                        daxPartition.State = (Partition.PartitionState)state;
                        daxPartition.Type = (Partition.PartitionType)type;
                        daxPartition.Mode = (Partition.PartitionMode)mode;
                        daxPartition.RefreshedTime = refreshedTime;
                    }
                }
            }
        }

        public void PopulateColumns()
        {
            PopulateColumnsData();
            PopulateColumnsCardinality();
            PopulateColumnsSegments();
            PopulateColumnsHierarchies();
        }

        protected void PopulateColumnsData()
        {
            const string QUERY_COLUMNS = @"
SELECT
    DIMENSION_NAME AS TABLE_NAME, 
    COLUMN_ID AS COLUMN_ID, 
    ATTRIBUTE_NAME AS COLUMN_NAME, 
    DATATYPE AS [Data Type],
    DICTIONARY_SIZE AS DICTIONARY_SIZE_BYTES,
    COLUMN_ENCODING AS COLUMN_ENCODING_INT
FROM  $SYSTEM.DISCOVER_STORAGE_TABLE_COLUMNS
WHERE COLUMN_TYPE = 'BASIC_DATA'";

            var cmd = CreateCommand(QUERY_COLUMNS);
            cmd.CommandTimeout = CommandTimeout;

            using (var rdr = cmd.ExecuteReader()) {
                while (rdr.Read()) {
                    string tableName = rdr.GetString(0);
                    string columnDmv1100Id = rdr.GetString(1);
                    string columnName = rdr.GetString(2);
                    string dataType = rdr.GetString(3);
                    long dictionarySize = (long)rdr.GetDecimal(4);
                    long columnEncodingInt = rdr.GetInt64(5);

                    Column daxColumn = GetDaxColumn(tableName, columnName);
                    daxColumn.SetDmv1100ColumnId(columnDmv1100Id);
                    daxColumn.DictionarySize = dictionarySize;
                    switch (columnEncodingInt) {
                        case 1: daxColumn.Encoding = "HASH"; break;
                        case 2: daxColumn.Encoding = "VALUE"; break;
                        default: daxColumn.Encoding = "UNKNOWN"; break;
                    }
                    if (string.IsNullOrEmpty(daxColumn.DataType)) {
                        // Use Tom.DataType to avoid typing errors in strings
                        switch (dataType) {
                            case "DBTYPE_I8": daxColumn.DataType = Tom.DataType.Int64.ToString(); break;
                            case "DBTYPE_R8": daxColumn.DataType = Tom.DataType.Double.ToString(); break;
                            case "DBTYPE_CY": daxColumn.DataType = Tom.DataType.Decimal.ToString(); break;
                            case "DBTYPE_BOOL": daxColumn.DataType = Tom.DataType.Boolean.ToString(); break;
                            case "DBTYPE_WSTR": daxColumn.DataType = Tom.DataType.String.ToString(); break;
                            case "DBTYPE_DATE": daxColumn.DataType = Tom.DataType.DateTime.ToString(); break;
                            default: Tom.DataType.Unknown.ToString(); break;
                        }
                    }
                }
            }
        }
        protected void PopulateColumnsCardinality()
        {
            const string QUERY_COLUMNS_CARDINALITY = @"
SELECT
    DIMENSION_NAME AS TABLE_NAME, 
    TABLE_ID AS COLUMN_HIERARCHY_ID,
    ROWS_COUNT - 3 AS COLUMN_CARDINALITY
FROM $SYSTEM.DISCOVER_STORAGE_TABLES
WHERE LEFT ( TABLE_ID, 2 ) = 'H$'
ORDER BY TABLE_ID";

            var cmd = CreateCommand(QUERY_COLUMNS_CARDINALITY);
            cmd.CommandTimeout = CommandTimeout;

            using (var rdr = cmd.ExecuteReader()) {
                while (rdr.Read()) {
                    string tableName = rdr.GetString(0);
                    string columnHierarchyId = rdr.GetString(1);
                    string columnDmv1100Id = columnHierarchyId.Substring(columnHierarchyId.LastIndexOf('$') + 1);
                    long columnCardinality = rdr.GetInt64(2);

                    Column daxColumn = GetDaxColumnDmv1100Id(tableName, columnDmv1100Id);
                    daxColumn.ColumnCardinality = columnCardinality;
                }
            }

        }

        protected void PopulateColumnsSegments()
        {
            const string QUERY_COLUMNS_SEGMENTS_BASE = @"
SELECT 
    DIMENSION_NAME AS TABLE_NAME, 
    PARTITION_NAME, 
    COLUMN_ID AS COLUMN_NAME, 
    SEGMENT_NUMBER, 
    TABLE_PARTITION_NUMBER, 
    RECORDS_COUNT AS SEGMENT_ROWS,
    USED_SIZE,
    COMPRESSION_TYPE,
    BITS_COUNT,
    BOOKMARK_BITS_COUNT,
    VERTIPAQ_STATE {0}
FROM $SYSTEM.DISCOVER_STORAGE_TABLE_COLUMN_SEGMENTS
WHERE RIGHT ( LEFT ( TABLE_ID, 2 ), 1 ) <> '$'";

            const string QUERY_COLUMNS_SEGMENTS_EXTENDED = @"
    , ISPAGEABLE
    , ISRESIDENT
    , TEMPERATURE
    , LAST_ACCESSED
";

            bool hasExtendedInfo = CheckExtendedColumnsSegmentInfo();

            string QUERY_COLUMNS_SEGMENTS = string.Format(QUERY_COLUMNS_SEGMENTS_BASE, hasExtendedInfo ? QUERY_COLUMNS_SEGMENTS_EXTENDED : "");
            var cmd = CreateCommand(QUERY_COLUMNS_SEGMENTS);
            cmd.CommandTimeout = CommandTimeout;

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string tableName = rdr.GetString(0);
                    string partitionName = rdr.GetString(1);
                    string columnDmv1100Id = rdr.GetString(2);
                    long segmentNumber = rdr.GetInt64(3);
                    long tablePartitionNumber = rdr.GetInt64(4);
                    long segmentRows = rdr.GetInt64(5);
                    long usedSize = (long)rdr.GetDecimal(6);
                    string compressionType = rdr.GetString(7);
                    long bitsCount = rdr.GetInt64(8);
                    long bookmarkBitsCount = rdr.GetInt64(9);
                    string vertipaqState = rdr.GetString(10);

                    bool? isPageable = null;
                    bool? isResident = null;
                    double? temperature = null;
                    DateTime? lastAccessed = null;

                    if (hasExtendedInfo)
                    {
                        if (!rdr.IsDBNull(11)) isPageable = rdr.GetBoolean(11);
                        if (!rdr.IsDBNull(12)) isResident = rdr.GetBoolean(12);
                        if (!rdr.IsDBNull(13)) temperature = rdr.GetDouble(13);
                        if (!rdr.IsDBNull(14)) lastAccessed = rdr.GetDateTime(14);
                    }

                    // Column daxColumn = GetDaxColumnDmv1100Id(tableName, columnDmv1100Id);
                    ColumnSegment daxColumnSegment = GetDaxColumnSegment(tableName, partitionName, columnDmv1100Id, segmentNumber, tablePartitionNumber);
                    daxColumnSegment.BitsCount = bitsCount;
                    daxColumnSegment.BookmarkBitsCount = bookmarkBitsCount;
                    daxColumnSegment.CompressionType = compressionType;
                    daxColumnSegment.SegmentRows = segmentRows;
                    daxColumnSegment.UsedSize = usedSize;
                    daxColumnSegment.VertipaqState = vertipaqState;
                    daxColumnSegment.IsPageable = isPageable;
                    daxColumnSegment.IsResident = isResident; 
                    daxColumnSegment.Temperature = temperature;
                    daxColumnSegment.LastAccessed = lastAccessed;
                }
            }

        }

        private bool CheckExtendedColumnsSegmentInfo()
        {
            // Retrieve all the columns to evaluate whether extended info is available
            const string QUERY_COLUMNS_PEEK_VERSION = @"
SELECT TOP 1 *
FROM $SYSTEM.DISCOVER_STORAGE_TABLE_COLUMN_SEGMENTS
";

            bool hasExtendedInfo = false;
            var cmdPeek = CreateCommand(QUERY_COLUMNS_PEEK_VERSION);
            cmdPeek.CommandTimeout = CommandTimeout;
            using (var rdr = cmdPeek.ExecuteReader())
            {
                if (rdr.Read())
                {
                    string[] extendedColumns = { "ISPAGEABLE", "ISRESIDENT", "TEMPERATURE", "LAST_ACCESSED" };
                    int foundColumns = 0;
                    for (int i = rdr.FieldCount - 1; i >= 0; i--)
                    {
                        if (Array.Find(extendedColumns, columnName => columnName == rdr.GetName(i).ToUpper()) != null)
                        {
                            foundColumns++;
                        };
                    }
                    hasExtendedInfo = (foundColumns == extendedColumns.Length);
                }
            }

            return hasExtendedInfo;
        }

        protected void PopulateColumnsHierarchies()
        {
            const string QUERY_HIERARCHIES = @"
SELECT 
    DIMENSION_NAME AS TABLE_NAME, 
    COLUMN_ID AS STRUCTURE_NAME,
    SEGMENT_NUMBER, 
    TABLE_PARTITION_NUMBER, 
    USED_SIZE,
    TABLE_ID AS COLUMN_HIERARCHY_ID
FROM $SYSTEM.DISCOVER_STORAGE_TABLE_COLUMN_SEGMENTS
WHERE LEFT ( TABLE_ID, 2 ) = 'H$'
  AND SEGMENT_NUMBER = 0";

            var cmd = CreateCommand(QUERY_HIERARCHIES);
            cmd.CommandTimeout = CommandTimeout;

            using (var rdr = cmd.ExecuteReader()) {
                while (rdr.Read()) {
                    string tableName = rdr.GetString(0);
                    string structureName = rdr.GetString(1);
                    long segmentNumber = rdr.GetInt64(2);
                    long tablePartitionNumber = rdr.GetInt64(3);
                    long usedSize = (long)rdr.GetDecimal(4);
                    string columnHierarchyId = rdr.GetString(5);
                    string columnDmv1100Id = columnHierarchyId.Substring(columnHierarchyId.LastIndexOf('$') + 1);

                    ColumnHierarchy daxColumnHierarchy = GetDaxColumnHierarchyDmv1100Id(tableName, columnDmv1100Id, structureName, tablePartitionNumber, segmentNumber);
                    daxColumnHierarchy.UsedSize = usedSize;
                }
            }
        }

        private Dictionary<int, string> GetUserHierarchiesNames()
        {
            const string QUERY_USER_HIERARCHIES = @"
SELECT 
    [ID] AS USER_HIERARCHY_ID,
    [NAME] AS USER_HIERARCHY_NAME
FROM $SYSTEM.TMSCHEMA_HIERARCHIES";

            var map = new Dictionary<int, string>();

            // Skip the PopulateRelationships task if the compatibility level is older than TOM
            if (DaxModel.CompatibilityLevel >= 1200) {
                var cmd = CreateCommand(QUERY_USER_HIERARCHIES);
                cmd.CommandTimeout = CommandTimeout;

                using (var rdr = cmd.ExecuteReader()) {
                    while (rdr.Read()) {
                        int userHierarchyId = (int)rdr.GetDecimal(0);
                        string userHierarchyName = rdr.GetString(1);
                        map.Add(userHierarchyId, userHierarchyName);
                    }
                }
            }

            return map;
        }

        public void PopulateUserHierarchies()
        {
            const string QUERY_USER_HIERARCHIES_SIZE = @"
SELECT 
    DIMENSION_NAME AS TABLE_NAME, 
    COLUMN_ID AS STRUCTURE_NAME,
    USED_SIZE AS USED_SIZE,
    TABLE_ID AS HIERARCHY_ID
FROM $SYSTEM.DISCOVER_STORAGE_TABLE_COLUMN_SEGMENTS
WHERE LEFT ( TABLE_ID, 2 ) = 'U$'";

            var mapUserHierarchyNames = GetUserHierarchiesNames();
            var cmd = CreateCommand(QUERY_USER_HIERARCHIES_SIZE);
            cmd.CommandTimeout = CommandTimeout;

            using (var rdr = cmd.ExecuteReader()) {
                // Reset size existing hierarchies
                foreach ( var t in this.DaxModel.Tables) {
                    foreach ( var uh in t.UserHierarchies ) {
                        uh.UsedSize = 0;
                    }
                }

                // Loop through the result of the DMV
                while (rdr.Read()) {
                    string tableName = rdr.GetString(0);
                    // string structureName = rdr.GetString(1);
                    long usedSize = (long)rdr.GetDecimal(2);
                    string userHierarchyFullId = rdr.GetString(3);
                    string userHierarchyId = userHierarchyFullId.Substring(userHierarchyFullId.LastIndexOf('$') + 1);

                    // Default for compatibility level < 1200
                    string userHierarchyName = userHierarchyId;

                    // Search visible name for compatibility level >= 1200
                    var openIdBracket = userHierarchyId.LastIndexOf('(');
                    var closeIdBracket = userHierarchyId.LastIndexOf(')');
                    if (openIdBracket >= 0 && closeIdBracket >= 0) {
                        var stringDmv1200Id = userHierarchyId.Substring(openIdBracket + 1, closeIdBracket - (openIdBracket + 1));
                        if (int.TryParse(stringDmv1200Id, out int intDmv1200Id)) {
                            mapUserHierarchyNames.TryGetValue(intDmv1200Id, out userHierarchyName);
                        }
                    }

                    UserHierarchy daxUserHierarchy = GetDaxUserHierarchy(tableName, userHierarchyName);
                    // We sum the size to the existing one
                    // The DMV returns multiple structures for each hierarchy (MULTI_LEVEL_ID, PARENT_POS, FIRST_CHILD_POS, CHILD_COUNT)
                    // UsedSize is reset to 0 before looping through DMV resuls
                    daxUserHierarchy.UsedSize += usedSize;

                }
            }
        }

        // TODO: bidi relationships have two sizes, FROM and TO must be set correctly
        private struct TableRelationshipIds
        {
            public int TableId;
            public int RelationshipId;

            public override bool Equals(object obj)
            {
                return (obj is TableRelationshipIds ids) 
                    && ids == this;
            }
            public override int GetHashCode()
            {
                return TableId * 100000 + RelationshipId;
            }
            public static bool operator ==(TableRelationshipIds x, TableRelationshipIds y)
            {
                return x.TableId == y.TableId && x.RelationshipId == y.RelationshipId;
            }
            public static bool operator !=(TableRelationshipIds x, TableRelationshipIds y)
            {
                return !(x == y);
            }
        }
            
        private Dictionary<TableRelationshipIds, long> GetRelationshipsSize()
        {
            const string QUERY_RELATIONSHIPS_SIZE = @"
SELECT 
    DIMENSION_NAME AS TABLE_NAME, 
    TABLE_ID AS RELATIONSHIP_ID,
    USED_SIZE AS USED_SIZE
FROM $SYSTEM.DISCOVER_STORAGE_TABLE_COLUMN_SEGMENTS
WHERE LEFT ( TABLE_ID, 2 ) = 'R$'";

            var map = new Dictionary<TableRelationshipIds, long>();
            var cmd = CreateCommand(QUERY_RELATIONSHIPS_SIZE);
            cmd.CommandTimeout = CommandTimeout;

            using (var rdr = cmd.ExecuteReader()) {
                while (rdr.Read()) {
                    string tableName = rdr.GetString(0);
                    string relationshipFullId = rdr.GetString(1);
                    long usedSize = (long)rdr.GetDecimal(2);

                    var daxTable = GetDaxTable(tableName);
                    var openIdBracket = relationshipFullId.LastIndexOf('(');
                    var closeIdBracket = relationshipFullId.LastIndexOf(')');
                    if (openIdBracket >= 0 && closeIdBracket >= 0) {
                        var stringId = relationshipFullId.Substring(openIdBracket + 1, closeIdBracket - (openIdBracket + 1));
                        TableRelationshipIds trIds;
                        trIds.TableId = daxTable.Dmv1200TableId;
                        if (int.TryParse(stringId, out trIds.RelationshipId)) {
                            map.Add(trIds, usedSize);
                        }
                    }
                }
            }

            return map;
        }

        public void PopulateLastDataUpdate()
        {
            const string QUERY_LASTUPDATE = @"
SELECT TOP 1 [LAST_DATA_UPDATE]
FROM $SYSTEM.MDSCHEMA_CUBES
ORDER BY [LAST_DATA_UPDATE] DESC";

            var cmd = CreateCommand(QUERY_LASTUPDATE);
            cmd.CommandTimeout = CommandTimeout;

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    DateTime lastDataRefresh = (DateTime)rdr.GetDateTime(0);
                    DaxModel.LastDataRefresh = DateTime.SpecifyKind(lastDataRefresh, DateTimeKind.Utc);
                }
            }
        }

        public void PopulateReferences()
        {
            // Skip the PopulateRelationships task if the compatibility level is older than TOM
            if (this.DaxModel.CompatibilityLevel < 1200)
            {
                return;
            }

            const string QUERY_CALC_DEPENDENCY = @"
SELECT DISTINCT 
    REFERENCED_OBJECT_TYPE, 
    REFERENCED_TABLE, 
    REFERENCED_OBJECT 
FROM $SYSTEM.DISCOVER_CALC_DEPENDENCY
";

            // There are bugs in the M engine reporting this possible error:
            // M Engine error: 'Microsoft.Data.Mashup; <pii>Token Literal expected. Start position: (4, 1). End position (4, 2).</pii>'.
            // We ignore this step if we find this type of error
            try
            {
                var cmd = CreateCommand(QUERY_CALC_DEPENDENCY);
                cmd.CommandTimeout = CommandTimeout;

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string objectType = rdr.GetString(0);
                        string tableName = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1);
                        string objectName = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2);

                        var table = DaxModel.Tables.Find(t => t.TableName.Name == tableName);
                        switch (objectType)
                        {
                            case "ATTRIBUTE_HIERARCHY":
                            case "CALC_COLUMN":
                            case "COLUMN":
                                // If there is a reference to a column, there is a reference to the table, too
                                if (table != null)
                                {
                                    table.IsReferenced = true;

                                    var column = table?.Columns.Find(c => c.ColumnName.Name == objectName);
                                    if (column != null) column.IsReferenced = true;
                                }
                                break;

                            case "CALC_TABLE":
                            case "TABLE":
                                if (table != null) table.IsReferenced = true;
                                break;

                            case "CALCULATION_ITEM":
                                // If there is a reference to a calculation item, there is a reference to the table corresponding to the calculation group
                                if (table != null)
                                {
                                    table.IsReferenced = true;

                                    var calculationItem = table.CalculationGroup?.CalculationItems.Find(ci => ci.ItemName.Name == objectName);
                                    if (calculationItem != null) calculationItem.IsReferenced = true;
                                }
                                break;

                            case "MEASURE":
                                // If there is a reference to a column, the table is not involved (the measure could move to another table)
                                var measure = table?.Measures.Find(c => c.MeasureName.Name == objectName);
                                if (measure != null) measure.IsReferenced = true;
                                break;
                            default:
                                // Unrecognized element, catch here only for debug purposes
                                break;
                        }
                    }
                }
            }
            catch (AdomdErrorResponseException ex)
            {
                // We ignore errors accessing this DMV
                Debug.WriteLine($"Ignored error in DISCOVER_CALC_DEPENDENCY: {ex.Message}");
            }
        }

        public void PopulateRelationships()
        {
            // Skip the PopulateRelationships task if the compatibility level is older than TOM
            if (this.DaxModel.CompatibilityLevel < 1200)
            {
                return;
            }
            const string QUERY_RELATIONSHIPS = @"
SELECT    
    [ID] AS [RelationshipID],
    [FromTableID],
    [FromColumnID],
    [FromCardinality] AS [FromCardinalityType],
    [ToTableID],
    [ToColumnID],
    [ToCardinality] AS [ToCardinalityType],
    [IsActive] AS [Active],
    [CrossFilteringBehavior],
    [JoinOnDateBehavior],
    [RelyOnReferentialIntegrity],
    [SecurityFilteringBehavior],
    [State]
FROM $SYSTEM.TMSCHEMA_RELATIONSHIPS";

            var mapRelationshipsSize = GetRelationshipsSize();
            var cmd = CreateCommand(QUERY_RELATIONSHIPS);
            cmd.CommandTimeout = CommandTimeout;

            using (var rdr = cmd.ExecuteReader()) {
                while (rdr.Read()) {
                    int relationshipDmv1200Id = (int)rdr.GetDecimal(0);
                    int fromTableDmv1200Id = (int)rdr.GetDecimal(1);
                    int fromColumnDmv1200Id = (int)rdr.GetDecimal(2);
                    // skip 3 and other columns - not used because other info should come from TOM
                    int toTableDmv1200Id = (int)rdr.GetDecimal(4);
                    int toColumnDmv1200Id = (int)rdr.GetDecimal(5);

                    Column fromColumn = GetDaxColumnDmv1200Id(fromTableDmv1200Id, fromColumnDmv1200Id);
                    Column toColumn = GetDaxColumnDmv1200Id(toTableDmv1200Id, toColumnDmv1200Id);
                    var relationship = DaxModel.Relationships.Where(r => r.FromColumn == fromColumn && r.ToColumn == toColumn).FirstOrDefault();
                    if (relationship == null)
                    {
                        // Create relationship
                        relationship = new Relationship(fromColumn, toColumn);
                        DaxModel.Relationships.Add(relationship);
                    }
                    if (relationship != null) {
                        relationship.Dmv1200RelationshipId = relationshipDmv1200Id;

                        // Check size From side
                        {
                            TableRelationshipIds tFromIds;
                            tFromIds.TableId = fromTableDmv1200Id;
                            tFromIds.RelationshipId = relationshipDmv1200Id;
                            if (mapRelationshipsSize.TryGetValue(tFromIds, out long usedSizeFrom)) {
                                relationship.UsedSizeFrom = usedSizeFrom;
                            }
                        }

                        // Check size To side
                        {
                            TableRelationshipIds tToIds;
                            tToIds.TableId = toTableDmv1200Id;
                            tToIds.RelationshipId = relationshipDmv1200Id;
                            if (mapRelationshipsSize.TryGetValue(tToIds, out long usedSizeTo)) {
                                relationship.UsedSizeTo = usedSizeTo;
                            }
                        }
                    }
                }
            }
        }
    }
}
