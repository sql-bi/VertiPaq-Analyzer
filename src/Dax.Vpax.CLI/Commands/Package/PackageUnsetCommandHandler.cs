namespace Dax.Vpax.CLI.Commands.Package;

internal sealed class PackageUnsetCommandHandler : CommandHandler
{
    public override Task<int> InvokeAsync(InvocationContext context)
    {
        var session = UserSession.Load();
        session.Package.Path = null;
        session.Save();

        return Task.FromResult(context.ExitCode);
    }
}
