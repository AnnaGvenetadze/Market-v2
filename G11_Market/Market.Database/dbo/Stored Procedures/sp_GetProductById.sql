CREATE   PROCEDURE dbo.sp_GetProductById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        RAISERROR('Product with Id %d was not found or has been deleted.', 16, 1, @Id);
        RETURN;
    END;

    SELECT Id, CategoryId, ProductName, Price, IsDeleted, CreatedDate, UpdatedDate
    FROM dbo.Products
    WHERE Id = @Id AND IsDeleted = 0;
END;