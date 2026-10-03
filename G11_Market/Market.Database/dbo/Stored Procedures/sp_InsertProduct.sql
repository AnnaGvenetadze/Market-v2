CREATE   PROCEDURE dbo.sp_InsertProduct
    @CategoryId INT,
    @ProductName NVARCHAR(100),
    @Price DECIMAL(18, 2),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @Price IS NULL OR @Price < 0 THROW 50000, 'Price must be nonnegative.', 1;
    IF NULLIF(LTRIM(RTRIM(@ProductName)), '') IS NULL THROW 50000, 'ProductName is required.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Id = @CategoryId AND IsDeleted = 0) THROW 50000, 'Active category not found.', 1;

    INSERT INTO dbo.Products (CategoryId, ProductName, Price, IsDeleted, CreatedDate)
    VALUES (@CategoryId, @ProductName, @Price, 0, GETUTCDATE());

    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;
GO