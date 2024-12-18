namespace Dax.Vpax.CLI;

internal sealed class UserSession
{
    private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".vpax", "cli-session.json");
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static UserSession Load()
    {
        UserSession? session = null;

        if (File.Exists(FilePath))
            session = JsonSerializer.Deserialize<UserSession>(json: File.ReadAllText(FilePath), JsonOptions);

        return session ?? new UserSession();
    }

    public static string? GetPackagePath() => Load().Package.Path;

    public Package Package { get; init; } = new();

    public void Save()
    {
        _ = Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(this, JsonOptions));
    }
}

internal sealed class Package
{
    public string? Path { get; set; }
}
