CREATE   PROCEDURE dbo.sp_UnassignEmployeeRole
    @EmployeeId INT,
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;
    EXEC dbo.sp_DeleteEmployeeRole @EmployeeId = @EmployeeId, @RoleId = @RoleId;
END;