CREATE PROCEDURE dbo.sp_InsertProduct
    @CategoryId INT,
    @ProductName NVARCHAR(100),
    @Price DECIMAL(18, 2),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Products
    (
        CategoryId,
        ProductName,
        Price,
        IsDeleted
    )
    VALUES
    (
        @CategoryId,
        @ProductName,
        @Price,
        0
    );

    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;
GO
