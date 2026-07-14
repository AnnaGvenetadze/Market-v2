CREATE PROCEDURE dbo.sp_DeleteRole
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Roles
    SET
        IsDeleted = 1,
        UpdateDate = GETDATE()
    WHERE Id = @Id
      AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Role with Id %d was not found or has already been deleted.', 16, 1, @Id);
    END;
END;
GO
