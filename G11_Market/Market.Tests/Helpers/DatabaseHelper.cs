using Microsoft.Data.SqlClient;

namespace Market.Tests.Helpers;

internal class DatabaseHelper
{
    private static string ConnectionString =>
ConfigurationManager.ConnectionString;

    private static readonly string SqlScriptsFolder =
        Path.Combine(AppContext.BaseDirectory, "SqlScripts");

    private static readonly string ClearDatabaseScriptPath =
        Path.Combine(SqlScriptsFolder, "ClearDatabase.txt");

    private static readonly string SeedDatabaseScriptPath =
        Path.Combine(SqlScriptsFolder, "SeedDatabase.txt");

    public static void ClearDatabase()
    {
        ExecuteScriptFromFile(ClearDatabaseScriptPath);
    }

    public static void SeedDatabase()
    {
        ExecuteScriptFromFile(SeedDatabaseScriptPath);
    }

    private static void ExecuteScriptFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"SQL script file was not found: {filePath}");
        }

        var script = File.ReadAllText(filePath);

        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        using var command = new SqlCommand(script, connection);
        command.ExecuteNonQuery();
    }
}