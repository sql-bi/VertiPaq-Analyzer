namespace Dax.Vpax.CLI;

internal sealed class UserSession
{
    private static readonly string s_filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".vpax", "cli-session.json");
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static UserSession Load()
    {
        UserSession? session = null;

        if (File.Exists(s_filePath))
            session = JsonSerializer.Deserialize<UserSession>(json: File.ReadAllText(s_filePath), s_jsonOptions);

        return session ?? new UserSession();
    }

    public static string? GetPackagePath() => Load().Package.Path;

    public Package Package { get; init; } = new();

    public void Save()
    {
        _ = Directory.CreateDirectory(Path.GetDirectoryName(s_filePath)!);
        File.WriteAllText(s_filePath, JsonSerializer.Serialize(this, s_jsonOptions));
    }
}

internal sealed class Package
{
    public string? Path { get; set; }
}
