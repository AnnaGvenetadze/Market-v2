using Microsoft.Data.SqlClient;

namespace Market.Tests.Helpers;

internal class DatabaseHelper
{
    private const string ConnectionString = "Server=.;Database=G11_Market_TEST;Integrated Security=True;TrustServerCertificate=True;";
    private const string ClearDatabaseScript = @"
        DELETE FROM Countries;
        DBCC CHECKIDENT ('Countries', RESEED, 0);";
    private const string SeedDatabaseScript = @"
        INSERT INTO Countries (Name, CountryCode)
        VALUES 
            ('Georgia', 'GEO'),
            ('Armenia', 'ARM'),
            ('Chad', 'CHA');";

    public static void ClearDatabase()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        var command = new SqlCommand(ClearDatabaseScript, connection);
        command.ExecuteNonQuery();
    }

    public static void SeedDatabase()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        var command = new SqlCommand(SeedDatabaseScript, connection);
        command.ExecuteNonQuery();
    }
}