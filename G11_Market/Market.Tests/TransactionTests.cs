using System.Data.Common;
using Market.Repositories;
using Market.Services.Interfaces;
using Market.Tests.Helpers;
using Microsoft.Data.SqlClient;

namespace Market.Tests;

public class TransactionTests
{
    private SqlConnection _connection;
    private IUnitOfWork _unitOfWork;

    [SetUp]
    public void Setup()
    {
        _connection = new SqlConnection(ConfigurationManager.ConnectionString);
        _connection.Open();
        CreateTempTable();
        _unitOfWork = UnitOfWorkFactory.Create(_connection);
    }

    [TearDown]
    public void TearDown()
    {
        if (_unitOfWork is IDisposable disposable)
        {
            disposable.Dispose();
        }
        _connection?.Dispose();
    }

    private void CreateTempTable()
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            IF OBJECT_ID('tempdb..#TransactionTestTable') IS NOT NULL
                DROP TABLE #TransactionTestTable;

            CREATE TABLE #TransactionTestTable (
                Id INT PRIMARY KEY, 
                Name NVARCHAR(100)
            );";
        command.ExecuteNonQuery();
    }

    private SqlTransaction? GetActiveTransaction()
    {
        var type = _unitOfWork.GetType();
        var field = type.GetField("_transaction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (SqlTransaction?)field?.GetValue(_unitOfWork);
    }

    private void InsertTestRow(int id, string name)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "INSERT INTO #TransactionTestTable (Id, Name) VALUES (@id, @name);";
        command.Transaction = GetActiveTransaction();
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", name);
        command.ExecuteNonQuery();
    }

    private bool CheckIfRowExists(int id)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM #TransactionTestTable WHERE Id = @id;";
        command.Transaction = GetActiveTransaction();
        command.Parameters.AddWithValue("@id", id);
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    [Test]
    public void UnitOfWorkFactory_Create_WithValidConnection_ReturnsInstance()
    {
        // Act
        var uow = UnitOfWorkFactory.Create(_connection);

        // Assert
        Assert.That(uow, Is.Not.Null);
        Assert.That(uow.GetType().Name, Is.EqualTo("UnitOfWork"));
    }

    [Test]
    public void UnitOfWorkFactory_Create_WithNullConnection_ThrowsArgumentNullException()
    {
        // Act And Assert
        var ex = Assert.Throws<ArgumentNullException>(() => UnitOfWorkFactory.Create(null!));
        Assert.That(ex.ParamName, Is.EqualTo("connection"));
    }

    [Test]
    public void Commit_RootTransaction_PersistsDataToDatabase()
    {
        // Arrange
        _unitOfWork.BeginTransaction();

        // Act
        InsertTestRow(1, "Database Engine A");
        _unitOfWork.Commit();

        // Assert
        Assert.That(CheckIfRowExists(1), Is.True);
    }

    [Test]
    public void Rollback_RootTransaction_RevertsAllChanges()
    {
        // Arrange
        _unitOfWork.BeginTransaction();

        // Act
        InsertTestRow(2, "Database Engine B");
        _unitOfWork.Rollback();

        // Assert
        Assert.That(CheckIfRowExists(2), Is.False);
    }

    [Test]
    public void Rollback_NestedTransaction_RollsBackOnlyNestedChanges()
    {
        // Arrange
        _unitOfWork.BeginTransaction();
        InsertTestRow(3, "Root Row");

        // Act
        _unitOfWork.BeginNestedTransaction();
        InsertTestRow(4, "Nested Row");
        _unitOfWork.Rollback();
        _unitOfWork.Commit();

        // Assert
        Assert.That(CheckIfRowExists(3), Is.True);
        Assert.That(CheckIfRowExists(4), Is.False);
    }

    [Test]
    public void RollbackToLastSavePoint_WithoutSavepoints_ThrowsInvalidOperationException()
    {
        // Arrange
        _unitOfWork.BeginTransaction();

        // Act And Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _unitOfWork.RollbackToLastSavePoint());
        Assert.That(ex.Message, Is.EqualTo("No savepoints available"));
    }

    [Test]
    public void RollbackToSavePoint_Named_RollsBackToSpecificState()
    {
        // Arrange
        _unitOfWork.BeginTransaction();
        InsertTestRow(5, "Root Row");
        _unitOfWork.BeginNestedTransaction();
        InsertTestRow(6, "First Nested Row");
        var savePointsStack = GetInternalSavePointsStack();
        string firstSavepointName = savePointsStack.Peek();
        _unitOfWork.BeginNestedTransaction();
        InsertTestRow(7, "Second Nested Row");

        // Act
        _unitOfWork.RollbackToSavePoint(firstSavepointName);
        _unitOfWork.Commit();
        _unitOfWork.Commit();

        // Assert
        Assert.That(CheckIfRowExists(5), Is.True);
        Assert.That(CheckIfRowExists(6), Is.False);
        Assert.That(CheckIfRowExists(7), Is.False);
    }

    [Test]
    public void BeginTransaction_WhenAlreadyActive_ThrowsInvalidOperationException()
    {
        // Arrange
        _unitOfWork.BeginTransaction();

        // Act And Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _unitOfWork.BeginTransaction());
        Assert.That(ex.Message, Is.EqualTo("Transaction already started"));
    }
}
