CREATE PROCEDURE dbo.sp_RestoreEmployee
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @Id IS NULL 
        THROW 50035, 'Employee ID is required.', 1;

    DECLARE @AccountId INT;
    SELECT @AccountId = AccountId FROM dbo.Employees WHERE Id = @Id;

    IF @AccountId IS NULL 
        THROW 50036, 'Employee not found.', 1;

    UPDATE dbo.Employees 
    SET IsDeleted = 0, UpdateDate = GETUTCDATE() 
    WHERE Id = @Id;

    UPDATE dbo.Accounts 
    SET IsDeleted = 0, UpdateDate = GETUTCDATE() 
    WHERE Id = @AccountId;
END;