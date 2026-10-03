CREATE   PROCEDURE dbo.sp_UpdateEmployeeRole
    @EmployeeId INT,
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @EmployeeId, @RoleId;
END;