CREATE PROCEDURE dbo.sp_AssignEmployeeRole
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
        FROM dbo.Employees
        WHERE Id = @EmployeeId
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Employee was not found.', 16, 1);
        RETURN -3;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Roles
        WHERE Id = @RoleId
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Role was not found.', 16, 1);
        RETURN -4;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.EmployeeRoles
        WHERE EmployeeId = @EmployeeId
          AND RoleId = @RoleId
    )
    BEGIN
        RAISERROR('Employee role already exists.', 16, 1);
        RETURN -5;
    END;

    INSERT INTO dbo.EmployeeRoles
    (
        EmployeeId,
        RoleId
    )
    VALUES
    (
        @EmployeeId,
        @RoleId
    );

    RETURN 0;
END;