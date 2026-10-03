
CREATE   PROCEDURE dbo.sp_DeleteCategory
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Categories
    SET IsDeleted = 1,
        UpdatedDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Category with Id %d was not found or is already deleted.', 16, 1, @Id);
    END;
END;