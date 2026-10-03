CREATE   PROCEDURE dbo.sp_GetAllEmployees
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, AccountId, ManagerEmployeeId, EmployeeCode, HireDate, IsDeleted, CreateDate, UpdateDate, FirstName, LastName, Email, PhoneNumber
    FROM dbo.Employees 
    WHERE IsDeleted = 0;
END;