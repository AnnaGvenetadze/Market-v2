using Dapper;

namespace Market.Tests;

public class CategoryRepositoryTests : BaseRepositoryTests
{
    [Test]
    public void Search_ShouldHandleDifferentIsDeletedExpressions()
    {
        // Arrange
        Connection.Execute(
            """
            UPDATE Categories
            SET IsDeleted = 1
            WHERE Id = @Id
            """,
            new { Id = DeleteTestId });

        // Act
        var defaultResult = UnitOfWork.CategoryRepository
            .Search(category =>
                category.Id == UpdateTestId ||
                category.Id == DeleteTestId)
            .ToList();

        var activeResult = UnitOfWork.CategoryRepository
            .Search(category =>
                (category.Id == UpdateTestId ||
                 category.Id == DeleteTestId) &&
                category.IsDeleted == false)
            .ToList();

        var deletedResult = UnitOfWork.CategoryRepository
            .Search(category =>
                (category.Id == UpdateTestId ||
                 category.Id == DeleteTestId) &&
                category.IsDeleted == true)
            .ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(
                defaultResult.Any(category =>
                    category.Id == UpdateTestId),
                Is.True);

            Assert.That(
                defaultResult.Any(category =>
                    category.Id == DeleteTestId),
                Is.False);

            Assert.That(
                activeResult.Any(category =>
                    category.Id == UpdateTestId),
                Is.True);

            Assert.That(
                activeResult.Any(category =>
                    category.Id == DeleteTestId),
                Is.False);

            Assert.That(
                deletedResult.Any(category =>
                    category.Id == DeleteTestId),
                Is.True);

            Assert.That(
                deletedResult.Any(category =>
                    category.Id == UpdateTestId),
                Is.False);
        });
    }
}