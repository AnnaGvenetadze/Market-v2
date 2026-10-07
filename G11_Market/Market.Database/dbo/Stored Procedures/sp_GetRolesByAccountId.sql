CREATE PROCEDURE dbo.sp_GetRolesByAccountId
    @AccountId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        r.Id,
        r.Name,
        r.Description,
        r.IsDeleted,
        r.CreateDate,
        r.UpdateDate
    FROM dbo.Roles AS r
    INNER JOIN dbo.EmployeeRoles AS er
        ON er.RoleId = r.Id
    INNER JOIN dbo.Employees AS e
        ON e.Id = er.EmployeeId
    INNER JOIN dbo.Accounts AS a
        ON a.Id = e.AccountId
    WHERE a.Id = @AccountId
      AND a.IsDeleted = 0
      AND a.IsActive = 1
      AND e.IsDeleted = 0
      AND r.IsDeleted = 0
    ORDER BY r.Name;
END;
GO