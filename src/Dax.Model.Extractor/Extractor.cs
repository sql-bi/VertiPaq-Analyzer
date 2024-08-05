using System;
using System.Data;
using System.Threading;
using Dax.Metadata;
using Dax.Model.Extractor.Data;
using Tom = Microsoft.AnalysisServices.Tabular;

namespace Dax.Model.Extractor;

public static class Extractor
{
    // TOFIX: (WIP) app and version should be passed as parameters
    private const string ExtractorApp = "(TODO)extractorApp";
    private const string ExtractorVersion = "(TODO)extractorVersion";

    public static Dax.Metadata.Model GetDaxModel(string connectionString, ExtractorSettings settings, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Value cannot be null or empty.", nameof(connectionString));
        if (settings is null) throw new ArgumentNullException(nameof(settings));

        using var server = new Tom.Server();
        server.Connect(connectionString);

        var databaseName = ConnectionStringUtils.GetInitialCatalog(connectionString, throwIfNotFound: true);
        return GetDaxModel(server, databaseName, settings, cancellationToken);
    }

    public static Dax.Metadata.Model GetDaxModel(Tom.Server server, string databaseName, ExtractorSettings settings, CancellationToken cancellationToken = default)
    {
        if (server is null) throw new ArgumentNullException(nameof(server));
        if (string.IsNullOrEmpty(databaseName)) throw new ArgumentNullException(nameof(databaseName));
        if (settings is null) throw new ArgumentNullException(nameof(settings));

        var database = server.Databases.GetByName(databaseName);
        var model = database.Model;
        using var connection = new TomConnection(server, databaseName);

        return GetDaxModel(model, connection, settings, cancellationToken);
    }

    public static Dax.Metadata.Model GetDaxModel(Tom.Model model, IDbConnection connection, ExtractorSettings settings, CancellationToken cancellationToken = default)
    {
        if (model is null) throw new ArgumentNullException(nameof(model));
        if (connection is null) throw new ArgumentNullException(nameof(connection));
        if (settings is null) throw new ArgumentNullException(nameof(settings));

        var daxModel = new Dax.Metadata.Model(extractorInfo.Name, extractorInfo.Version, extractorApp, extractorVersion);

        var daxModel = TomExtractor.GetDaxModel(model, ExtractorApp, ExtractorVersion);
        var serverName = model.Server?.Name ?? ConnectionStringUtils.GetDataSource(connection.ConnectionString, throwIfNotFound: true);
        var databaseName = model.Name;

        DmvExtractor.PopulateFromDmv(daxModel, connection, serverName, databaseName, ExtractorApp, ExtractorVersion);

        //var infoExtractor = new InfoExtractor(connection, settings, daxModel);
        //infoExtractor.Populate();

        if (settings.StatisticsEnabled)
        {
            var analyzeDirectQuery = settings.DirectQueryMode != DirectQueryExtractionMode.None;
            StatExtractor.UpdateStatisticsModel(daxModel, connection, settings.ReferentialIntegrityViolationSamples, analyzeDirectQuery, settings.DirectLakeMode);

            // If model has any DL partitions and we have forced all columns into memory then re-run the DMVs to update the data with the new values after everything has been transcoded.
            if (settings.DirectLakeMode > DirectLakeExtractionMode.ResidentOnly && daxModel.HasDirectLakePartitions())
                DmvExtractor.PopulateFromDmv(daxModel, connection, serverName, databaseName, ExtractorApp, ExtractorVersion);
        }

        return daxModel;
    }

    public static Tom.Model GetTomModel(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Value cannot be null or empty.", nameof(connectionString));

        using var server = new Tom.Server();
        server.Connect(connectionString);

        var databaseName = ConnectionStringUtils.GetInitialCatalog(connectionString, throwIfNotFound: true);
        var database = server.Databases.GetByName(databaseName);
        var model = database.Model;

        return model;
    }
}
