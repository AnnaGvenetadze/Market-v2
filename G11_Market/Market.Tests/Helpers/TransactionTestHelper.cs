using System.Reflection;
using Market.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace Market.Tests.Helpers;

public static class TransactionTestHelper
{
    public static SqlTransaction? GetActiveTransaction(IUnitOfWork unitOfWork)
    {
        var type = unitOfWork.GetType();
        var field = type.GetField("_transaction", BindingFlags.NonPublic | BindingFlags.Instance);

        return (SqlTransaction?)field?.GetValue(unitOfWork);
    }

    public static Stack<string> GetInternalSavePointsStack(IUnitOfWork unitOfWork)
    {
        var type = unitOfWork.GetType();
        var field = type.GetField("_transactionSavePoints", BindingFlags.NonPublic | BindingFlags.Instance);

        return (Stack<string>)field!.GetValue(unitOfWork)!;
    }

    public static void InsertTestRecord(SqlConnection connection, SqlTransaction? transaction, int id, string name)
    {
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SET IDENTITY_INSERT Categories ON;
            INSERT INTO Categories (Id, CategoryName) VALUES (@id, @name);
            SET IDENTITY_INSERT Categories OFF;";

        command.Transaction = transaction;
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", name);
        command.ExecuteNonQuery();
    }

    public static bool CheckIfRecordExists(SqlConnection connection, SqlTransaction? transaction, int id)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM Categories WHERE Id = @id;";
        command.Transaction = transaction;
        command.Parameters.AddWithValue("@id", id);

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }
}