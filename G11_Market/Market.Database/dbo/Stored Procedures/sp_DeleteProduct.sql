CREATE   PROCEDURE dbo.sp_DeleteProduct
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Products
    SET IsDeleted = 1,
        UpdatedDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Product with Id %d was not found or has been deleted.', 16, 1, @Id);
    END;
END;
