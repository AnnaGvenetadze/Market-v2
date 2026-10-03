CREATE   PROCEDURE dbo.sp_RestoreCategory
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Categories
    SET IsDeleted = 0, UpdatedDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 1;

    IF @@ROWCOUNT = 0
        RAISERROR('Category with Id %d was not found or is not deleted.', 16, 1, @Id);
END;
GO