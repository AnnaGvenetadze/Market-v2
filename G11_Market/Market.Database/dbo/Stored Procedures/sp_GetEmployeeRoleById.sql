CREATE   PROCEDURE dbo.sp_GetEmployeeRoleById
    @EmployeeId INT = NULL,
    @RoleId INT = NULL,
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EmployeeId, RoleId
    FROM dbo.EmployeeRoles
    WHERE (@EmployeeId IS NULL OR EmployeeId = @EmployeeId)
      AND (@RoleId IS NULL OR RoleId = @RoleId);
END;