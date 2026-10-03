
CREATE   PROCEDURE dbo.sp_GetEmployeeByAcountId
    @AccountId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT emp.Id AS EmployeeId, emp.Email, emp.FirstName, emp.LastName, emp.PhoneNumber, emp.EmployeeCode
    FROM dbo.Employees emp JOIN dbo.Accounts acc ON acc.Id = emp.AccountId
    WHERE emp.AccountId = @AccountId AND emp.IsDeleted = 0 AND acc.IsDeleted = 0;
END;
go