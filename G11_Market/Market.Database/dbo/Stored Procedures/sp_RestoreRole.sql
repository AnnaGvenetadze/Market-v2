CREATE PROCEDURE dbo.sp_RestoreRole
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Roles
    SET IsDeleted = 0, UpdateDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 1;

    IF @@ROWCOUNT = 0
        RAISERROR('Role with Id %d was not found or is not deleted.', 16, 1, @Id);
END;
GO
