using System.Collections.Generic;
using System;
using System.Data;
using Microsoft.AnalysisServices;

namespace Dax.Model.Extractor;

internal sealed class InfoExtractor
{
    private readonly IDbConnection _connection;
    private readonly ExtractorSettings _settings;
    private readonly Metadata.Model _model;

    public InfoExtractor(IDbConnection connection, ExtractorSettings settings, Dax.Metadata.Model model)
    {
        _connection = connection;
        _settings = settings;
        _model = model;
    }

    public void Populate()
    {
        PopulateModel(); // TODO: can we remove this? (see TomExtractor.PopulateModel)
        //PopulateTables();
    }

    private IDbCommand CreateCommand(string commandText)
    {
        var command = _connection.CreateCommand();
        command.CommandTimeout = _settings.CommandTimeout;
        command.CommandText = commandText;
        return command;
    }

    private void PopulateModel()
    {
        // Skip if the compatibility level is older than TOM
        if (_model.CompatibilityLevel < 1200)
            return;

        const string CommandText =
"""
        EVALUATE INFO.MODEL()
""";
        using var command = CreateCommand(CommandText);
        using var reader = command.ExecuteReader();

        if (!reader.Read())
            throw new InvalidOperationException("No model information found");

        var defautModeOrdinal = reader.GetFieldOrdinal("DefaultMode");
        var cultureOrdinal = reader.GetFieldOrdinal("Culture");

        _model.DefaultMode = reader.GetEnumInt32<Dax.Metadata.Partition.PartitionMode>(defautModeOrdinal);
        _model.Culture = reader.GetString(cultureOrdinal);

        // We do not expect multiple models in a single database
        if (reader.Read())
            throw new InvalidOperationException("Multiple models found in the database");
    }

//    public void PopulateTables()
//    {
//        const string QUERY_TABLES = @"
//SELECT 
//    DIMENSION_NAME AS TABLE_NAME, 
//    TABLE_ID       AS TABLE_ID,
//    ROWS_COUNT     AS ROWS_IN_TABLE,
//    RIVIOLATION_COUNT AS RI_VIOLATION_COUNT
//FROM  $SYSTEM.DISCOVER_STORAGE_TABLES
//WHERE RIGHT ( LEFT ( TABLE_ID, 2 ), 1 ) <> '$'
//ORDER BY DIMENSION_NAME";

//        const string CommandText =
//"""
//    EVALUATE INFO.TABLES()
//""";
//        using var command = CreateCommand(CommandText);
//        using var reader = command.ExecuteReader();

//        var nameOrdinal = reader.GetFieldOrdinal("Name");
//        var cultureOrdinal = reader.GetFieldOrdinal("Culture");

//        while (reader.Read())
//        {
//            string tableName = reader.GetString(nameOrdinal);
//            string tableId = reader.GetString(1);
//            long rowsCount = reader.GetInt64(2);
//            long referentialIntegrityViolationCount = reader.GetInt64(3);

//            //Table daxTable = GetDaxTable(tableName);
//            daxTable.SetDmv1100TableId(tableId);
//            //daxTable.RowsCount = rowsCount;
//            //daxTable.ReferentialIntegrityViolationCount = referentialIntegrityViolationCount;
//        }
//    }
}

internal static class DataReaderExtensions
{
    public static int GetFieldOrdinal(this IDataReader reader, string name)
    {
        try
        {
            return reader.GetOrdinal($"[{name}]");
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException($"Field '{name}' not found", ex);
        }
    }

    public static T GetEnumInt32<T>(this IDataReader reader, int ordinal) where T : Enum
    {
        // TODO: review this method for performance

        var value = reader.GetValue(ordinal);

        if (value is int)
        {
            if (Enum.IsDefined(typeof(T), value))
                return (T)value;
        }
        else if (value is long longValue && longValue >= int.MinValue && longValue <= int.MaxValue)
        {
            object intValue = (int)longValue;
            if (Enum.IsDefined(typeof(T), intValue))
                return (T)intValue;
        }

        throw new InvalidOperationException($"Invalid value '{value}' for enum '{typeof(T).Name}'");
    }

    public static IEnumerable<T> Select<T>(this IDataReader reader, Func<IDataReader, T> selector)
    {
        while (reader.Read())
        {
            yield return selector(reader);
        }

        reader.Close();
    }
}
