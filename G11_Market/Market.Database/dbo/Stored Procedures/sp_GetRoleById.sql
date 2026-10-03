CREATE   PROCEDURE dbo.sp_GetRoleById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        RAISERROR('Role with Id %d was not found or has been deleted.', 16, 1, @Id);
        RETURN;
    END;

    SELECT Id, Name, Description, IsDeleted, CreateDate, UpdateDate
    FROM dbo.Roles
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO
