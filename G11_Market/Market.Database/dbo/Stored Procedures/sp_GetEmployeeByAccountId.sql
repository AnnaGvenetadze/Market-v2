CREATE   PROCEDURE dbo.sp_GetEmployeeByAccountId
    @AccountId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT emp.Id, emp.AccountId, emp.ManagerEmployeeId, emp.EmployeeCode, emp.HireDate, emp.IsDeleted, emp.CreateDate, emp.UpdateDate, emp.FirstName, emp.LastName, emp.Email, emp.PhoneNumber
    FROM dbo.Employees emp 
    JOIN dbo.Accounts acc ON acc.Id = emp.AccountId
    WHERE emp.AccountId = @AccountId AND emp.IsDeleted = 0 AND acc.IsDeleted = 0;
END;