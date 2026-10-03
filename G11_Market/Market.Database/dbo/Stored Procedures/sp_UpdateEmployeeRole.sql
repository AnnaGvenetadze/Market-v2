CREATE PROCEDURE dbo.sp_UpdateEmployeeRole
    @EmployeeId INT,
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @EmployeeId IS NULL THROW 50000, 'EmployeeId is required.', 1;
    IF @RoleId IS NULL THROW 50000, 'RoleId is required.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Id = @EmployeeId AND IsDeleted = 0)
        THROW 50000, 'Employee not found or is inactive.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Id = @RoleId AND IsDeleted = 0)
        THROW 50000, 'Role not found or is inactive.', 1;

    IF EXISTS (SELECT 1 FROM dbo.EmployeeRoles WHERE EmployeeId = @EmployeeId)
    BEGIN
        UPDATE dbo.EmployeeRoles
        SET RoleId = @RoleId
        WHERE EmployeeId = @EmployeeId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.EmployeeRoles (EmployeeId, RoleId)
        VALUES (@EmployeeId, @RoleId);
    END
END;