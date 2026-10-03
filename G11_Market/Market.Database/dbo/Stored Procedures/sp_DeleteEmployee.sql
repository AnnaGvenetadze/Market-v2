CREATE   PROCEDURE dbo.sp_DeleteEmployee
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    IF @Id IS NULL THROW 50035, 'Employee ID is required for deletion.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Id = @Id AND IsDeleted = 0) THROW 50036, 'Employee not found or has already been deleted.', 1;

    UPDATE dbo.Employees
    SET IsDeleted = 1,
        UpdateDate = GETUTCDATE()
    WHERE Id = @Id;
END;