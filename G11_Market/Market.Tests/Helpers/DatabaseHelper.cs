using Microsoft.Data.SqlClient;

namespace Market.Tests.Helpers;

internal class DatabaseHelper
{
    private const string ConnectionString = "Server=localhost;Database=MarketDB_Test;Trusted_Connection=True;TrustServerCertificate=True;";
        //= "Server=.;Database=G11_Market_Test;Trusted_Connection=True;TrustServerCertificate=True;";

    private const string ClearDatabaseScript = @"
        -- Self-referencing tables: break self FK links first
        UPDATE Employees
        SET ManagerEmployeeId = NULL;

        UPDATE Categories
        SET ParentId = NULL;

        -- Transaction / movement tables
        DELETE FROM StockMovements;
        DELETE FROM SaleItems;
        DELETE FROM Sales;

        -- Product-related child tables
        DELETE FROM ProductAttributeValues;
        DELETE FROM CategoryAttributes;

        -- Employee/account child tables
        DELETE FROM InventoryManagerDetails;
        DELETE FROM EmployeeRoles;
        DELETE FROM CorporateClientDetails;
        DELETE FROM Clients;

        -- Main entity tables
        DELETE FROM Employees;
        DELETE FROM Products;

        -- Parent/reference tables
        DELETE FROM Accounts;
        DELETE FROM Roles;
        DELETE FROM ClientTypes;
        DELETE FROM Attributes;
        DELETE FROM Categories;
        DELETE FROM Cities;
        DELETE FROM Countries;

        -- Reseed identity tables
        DBCC CHECKIDENT ('StockMovements', RESEED, 0);
        DBCC CHECKIDENT ('SaleItems', RESEED, 0);
        DBCC CHECKIDENT ('Sales', RESEED, 0);
        DBCC CHECKIDENT ('Products', RESEED, 0);
        DBCC CHECKIDENT ('Employees', RESEED, 0);
        DBCC CHECKIDENT ('Clients', RESEED, 0);
        DBCC CHECKIDENT ('Accounts', RESEED, 0);
        DBCC CHECKIDENT ('Roles', RESEED, 0);
        DBCC CHECKIDENT ('ClientTypes', RESEED, 0);
        DBCC CHECKIDENT ('Attributes', RESEED, 0);
        DBCC CHECKIDENT ('Categories', RESEED, 0);
        DBCC CHECKIDENT ('Cities', RESEED, 0);
        DBCC CHECKIDENT ('Countries', RESEED, 0);
    ";
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