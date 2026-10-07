CREATE PROCEDURE dbo.sp_RestoreProduct
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Products
    SET IsDeleted = 0, UpdatedDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 1;

    IF @@ROWCOUNT = 0
        RAISERROR('Product with Id %d was not found or is not deleted.', 16, 1, @Id);
END;
GO