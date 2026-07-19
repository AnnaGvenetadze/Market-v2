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
        DatabaseHelper.ClearDatabase();
        _connection = new SqlConnection(ConfigurationManager.ConnectionString);
        _connection.Open();
        _unitOfWork = UnitOfWorkFactory.Create(_connection);
    }

    [TearDown]
    public void TearDown()
    {
        _connection?.Dispose();
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
        var transaction = TransactionTestHelper.GetActiveTransaction(_unitOfWork);

        // Act
        TransactionTestHelper.InsertTestRecord(_connection, transaction, 1, "Database Engine A");

        // Assert
        Assert.That(TransactionTestHelper.CheckIfRecordExists(_connection, transaction, 1), Is.True,
            "Record should be visible inside the active transaction.");
        _unitOfWork.Commit();
        Assert.That(TransactionTestHelper.CheckIfRecordExists(_connection, null, 1), Is.True,
            "Record should persist in the database after commit.");
    }

    [Test]
    public void Commit_NestedTransaction_DoesNotPersistUntilRootCommits()
    {
        // Arrange
        _unitOfWork.BeginTransaction();
        var transaction = TransactionTestHelper.GetActiveTransaction(_unitOfWork);
        TransactionTestHelper.InsertTestRecord(_connection, transaction, 3, "Root Row");

        // Act
        _unitOfWork.BeginNestedTransaction();
        TransactionTestHelper.InsertTestRecord(_connection, transaction, 4, "Nested Row");
        _unitOfWork.Commit();
        _unitOfWork.Rollback();

        // Assert
        Assert.That(TransactionTestHelper.CheckIfRecordExists(_connection, null, 3), Is.False);
        Assert.That(TransactionTestHelper.CheckIfRecordExists(_connection, null, 4), Is.False);
    }

    [Test]
    public void BeginTransaction_WhenAlreadyActive_ThrowsInvalidOperationException()
    {
        _unitOfWork.BeginTransaction();

        var ex = Assert.Throws<InvalidOperationException>(() => _unitOfWork.BeginTransaction());
        Assert.That(ex.Message, Is.EqualTo("Transaction already started"));
    }

    [Test]
    public void RollbackToLastSavePoint_WithoutSavepoints_ThrowsInvalidOperationException()
    {
        _unitOfWork.BeginTransaction();

        var ex = Assert.Throws<InvalidOperationException>(() => _unitOfWork.RollbackToLastSavePoint());
        Assert.That(ex.Message, Is.EqualTo("No savepoints available"));
    }

    [Test]
    public void RollbackToSavePoint_WithInvalidName_ThrowsArgumentException()
    {
        _unitOfWork.BeginTransaction();
        _unitOfWork.BeginNestedTransaction();

        var ex = Assert.Throws<ArgumentException>(() => _unitOfWork.RollbackToSavePoint("invalid_savepoint_name"));
        Assert.That(ex.ParamName, Is.EqualTo("savePoint"));
    }

    [Test]
    public void Repositories_WhenAccessed_AreSuccessfullyLazyLoaded()
    {
        // Assert that the Lazy instances resolve correctly without throwing exceptions
        Assert.That(_unitOfWork.CategoryRepository, Is.Not.Null);
        Assert.That(_unitOfWork.ProductRepository, Is.Not.Null);
        Assert.That(_unitOfWork.EmployeeRepository, Is.Not.Null);
    }

    [Test]
    public void Rollback_RootTransaction_RevertsAllChanges()
    {
        // Arrange
        _unitOfWork.BeginTransaction();
        var transaction = TransactionTestHelper.GetActiveTransaction(_unitOfWork);

        // Act
        TransactionTestHelper.InsertTestRecord(_connection, transaction, 2, "Database Engine B");

        // Assert: Record is visible inside the transaction before rollback
        Assert.That(TransactionTestHelper.CheckIfRecordExists(_connection, transaction, 2), Is.True,
            "Record should be visible inside the active transaction.");

        _unitOfWork.Rollback();

        // Assert: Record is completely gone after rollback
        Assert.That(TransactionTestHelper.CheckIfRecordExists(_connection, null, 2), Is.False,
            "Record should not exist in the database after rollback.");
    }

    [Test]
    public void Rollback_NestedTransaction_RollsBackOnlyNestedChanges()
    {
        // Arrange
        _unitOfWork.BeginTransaction();
        var transaction = TransactionTestHelper.GetActiveTransaction(_unitOfWork);

        TransactionTestHelper.InsertTestRecord(_connection, transaction, 3, "Root Row");

        // Act
        _unitOfWork.BeginNestedTransaction();
        TransactionTestHelper.InsertTestRecord(_connection, transaction, 4, "Nested Row");
        _unitOfWork.Rollback();
        _unitOfWork.Commit();

        // Assert
        Assert.That(TransactionTestHelper.CheckIfRecordExists(_connection, null, 3), Is.True);
        Assert.That(TransactionTestHelper.CheckIfRecordExists(_connection, null, 4), Is.False);
    }

    [Test]
    public void RollbackToSavePoint_Named_RollsBackToSpecificState()
    {
        // Arrange
        _unitOfWork.BeginTransaction();
        var transaction = TransactionTestHelper.GetActiveTransaction(_unitOfWork);

        TransactionTestHelper.InsertTestRecord(_connection, transaction, 5, "Root Row");

        _unitOfWork.BeginNestedTransaction();
        TransactionTestHelper.InsertTestRecord(_connection, transaction, 6, "First Nested Row");

        var savePointsStack = TransactionTestHelper.GetInternalSavePointsStack(_unitOfWork);
        string firstSavepointName = savePointsStack.Peek();

        _unitOfWork.BeginNestedTransaction();
        TransactionTestHelper.InsertTestRecord(_connection, transaction, 7, "Second Nested Row");

        // Act
        _unitOfWork.RollbackToSavePoint(firstSavepointName);
        _unitOfWork.Commit();
        _unitOfWork.Commit();

        // Assert
        Assert.That(TransactionTestHelper.CheckIfRecordExists(_connection, null, 5), Is.True);
        Assert.That(TransactionTestHelper.CheckIfRecordExists(_connection, null, 6), Is.False);
        Assert.That(TransactionTestHelper.CheckIfRecordExists(_connection, null, 7), Is.False);
    }
}
