
CREATE   PROCEDURE dbo.sp_AssignEmployeeRole
    @EmployeeId INT,
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DummyId INT;
    EXEC dbo.sp_InsertEmployeeRole @EmployeeId = @EmployeeId, @RoleId = @RoleId, @Id = @DummyId OUTPUT;
END;