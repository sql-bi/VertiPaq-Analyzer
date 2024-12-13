using static Dax.Vpax.CLI.Commands.ExportCommandOptions;

namespace Dax.Vpax.CLI.Commands;

internal sealed class ExportCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context) => throw new NotSupportedException("Use InvokeAsync instead.");

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        // TODO: forward cancellation token to vertipaq-analyzer extractor
        var cancellationToken = context.GetCancellationToken();
        var extractorAppName = ThisAssembly.AssemblyName;
        var extractorAppVersion = ThisAssembly.AssemblyInformationalVersion;

        var path = context.ParseResult.GetValueForArgument(PathArgument);
        var connectionString = context.ParseResult.GetValueForArgument(ConnectionStringArgument);
        var overwrite = context.ParseResult.GetValueForOption(OverwriteOption);
        var excludeTom = context.ParseResult.GetValueForOption(ExcludeTomOption);
        var excludeVpa = context.ParseResult.GetValueForOption(ExcludeVpaOption);
        var directQueryMode = context.ParseResult.GetValueForOption(DirectQueryModeOption);
        var directLakeMode = context.ParseResult.GetValueForOption(DirectLakeModeOption);
        var columnBatchSize = context.ParseResult.GetValueForOption(ColumnBatchSizeOption);

        using var vpaxStream = new MemoryStream();

        // TODO: improve logging and support platform-specific commands such as those used in Azure DevOps and GitHub
        context.Console.Out.Write("Extracting VPAX metadata...");
        {
            var daxModel = TomExtractor.GetDaxModel(
                connectionString: connectionString,
                applicationName: extractorAppName,
                applicationVersion: extractorAppVersion,
                readStatisticsFromData: true,
                sampleRows: 0, // not applicable for VPAX export
                analyzeDirectQuery: directQueryMode != DirectQueryExtractionMode.None,
                analyzeDirectLake: directLakeMode,
                statsColumnBatchSize: columnBatchSize
                );

            var vpaModel = excludeVpa ? null : new ViewVpaExport.Model(daxModel);
            var tomDatabase = excludeTom ? null : TomExtractor.GetDatabase(connectionString);

            VpaxTools.ExportVpax(vpaxStream, daxModel, vpaModel, tomDatabase);
        }
        context.Console.Out.WriteLine("done.");
        context.Console.Out.Write("Exporting VPAX file...");
        {
            var mode = overwrite ? FileMode.Create : FileMode.CreateNew;
            using var fileStream = new FileStream(path, mode, FileAccess.Write, FileShare.Read);
            await vpaxStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
        }
        context.Console.Out.WriteLine("done.");

        return context.ExitCode;
    }
}
