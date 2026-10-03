CREATE   PROCEDURE dbo.sp_GetAllEmployeeRoles
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EmployeeId, RoleId
    FROM dbo.EmployeeRoles;
END;