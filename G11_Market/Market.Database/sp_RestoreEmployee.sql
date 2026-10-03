CREATE PROCEDURE dbo.sp_RestoreEmployee
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Employees
    SET IsDeleted = 0, UpdateDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 1;

    IF @@ROWCOUNT = 0
        RAISERROR('Employee with Id %d was not found or is not deleted.', 16, 1, @Id);
END;
GO
