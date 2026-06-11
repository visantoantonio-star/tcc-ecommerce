using System.Text.Json;
using Npgsql;

Console.WriteLine("DbMigrate: applying SQL migration file to database...");

var repoRoot = Directory.GetCurrentDirectory();
// Find appsettings.json by walking up directories from current working dir
string? projectRoot = null;
var dir = new DirectoryInfo(repoRoot);
while (dir != null)
{
    var candidate = Path.Combine(dir.FullName, "appsettings.json");
    if (File.Exists(candidate))
    {
        projectRoot = dir.FullName;
        break;
    }
    dir = dir.Parent;
}

if (projectRoot == null)
{
    Console.Error.WriteLine("appsettings.json not found in parent folders.");
    return 1;
}

var appsettingsPath = Path.Combine(projectRoot, "appsettings.json");
if (!File.Exists(appsettingsPath))
{
    Console.Error.WriteLine($"appsettings.json not found at {appsettingsPath}");
    return 1;
}

var json = File.ReadAllText(appsettingsPath);
using var doc = JsonDocument.Parse(json);
if (!doc.RootElement.TryGetProperty("ConnectionStrings", out var connSection) ||
    !connSection.TryGetProperty("DefaultConnection", out var connValue))
{
    Console.Error.WriteLine("ConnectionStrings.DefaultConnection not found in appsettings.json");
    return 1;
}

var connString = connValue.GetString();
if (string.IsNullOrEmpty(connString))
{
    Console.Error.WriteLine("Connection string is empty");
    return 1;
}

var sqlFile = Path.Combine(projectRoot, "Migrations", "20260608_0002_orderitems_productid_nullable.up.sql");
if (!File.Exists(sqlFile))
{
    Console.Error.WriteLine($"Migration SQL file not found: {sqlFile}");
    return 1;
}

var sql = File.ReadAllText(sqlFile);

Console.WriteLine("Using connection string from appsettings.json (masked)...");
// Do not print connection string

try
{
    await using var conn = new NpgsqlConnection(connString);
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand(sql, conn);
    cmd.CommandTimeout = 600;
    var affected = await cmd.ExecuteNonQueryAsync();
    Console.WriteLine("Migration executed successfully.");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Migration failed: {ex.Message}");
    return 2;
}
