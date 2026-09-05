CREATE PROCEDURE dbo.sp_UpdateRole
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Roles
    SET
        Name = @Name,
        Description = @Description,
        UpdateDate = GETDATE()
    WHERE Id = @Id
      AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Role with Id %d was not found or has been deleted.', 16, 1, @Id);
    END;
END;
GO
