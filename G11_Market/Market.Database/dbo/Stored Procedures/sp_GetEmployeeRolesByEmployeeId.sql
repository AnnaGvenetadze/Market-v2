CREATE   PROCEDURE dbo.sp_GetEmployeeRolesByEmployeeId
    @EmployeeId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT r.Id, r.Name, r.Description
    FROM dbo.Roles AS r
    INNER JOIN dbo.EmployeeRoles AS er ON r.Id = er.RoleId
    WHERE er.EmployeeId = @EmployeeId AND r.IsDeleted = 0;
END;