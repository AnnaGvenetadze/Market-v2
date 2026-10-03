CREATE   PROCEDURE dbo.sp_GetEmployeeById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL THROW 50035, 'Employee ID is required.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Id = @Id AND IsDeleted = 0) 
        THROW 50036, 'Employee not found or has been deleted.', 1;

    SELECT Id, AccountId, ManagerEmployeeId, EmployeeCode, HireDate, IsDeleted, CreateDate, UpdateDate, FirstName, LastName, Email, PhoneNumber
    FROM dbo.Employees 
    WHERE Id = @Id AND IsDeleted = 0;
END;