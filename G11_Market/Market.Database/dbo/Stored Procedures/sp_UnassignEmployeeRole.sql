
CREATE   PROCEDURE dbo.sp_UnassignEmployeeRole
    @EmployeeId INT,
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @EmployeeId IS NULL
    BEGIN
        RAISERROR('EmployeeId is required.', 16, 1);
        RETURN -1;
    END;

    IF @RoleId IS NULL
    BEGIN
        RAISERROR('RoleId is required.', 16, 1);
        RETURN -2;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.EmployeeRoles
        WHERE EmployeeId = @EmployeeId
          AND RoleId = @RoleId
    )
    BEGIN
        RAISERROR('Employee role was not found.', 16, 1);
        RETURN -3;
    END;

    DELETE FROM dbo.EmployeeRoles
    WHERE EmployeeId = @EmployeeId
      AND RoleId = @RoleId;

    RETURN 0;
END;