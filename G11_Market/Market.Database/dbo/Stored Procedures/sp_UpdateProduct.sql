CREATE PROCEDURE dbo.sp_UpdateProduct
    @Id INT,
    @CategoryId INT,
    @ProductName NVARCHAR(100),
    @Price DECIMAL(18, 2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Products
    SET
        CategoryId = @CategoryId,
        ProductName = @ProductName,
        Price = @Price,
        UpdatedDate = GETDATE()
    WHERE Id = @Id
      AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Product with Id %d was not found or has been deleted.', 16, 1, @Id);
    END;
END;
GO
