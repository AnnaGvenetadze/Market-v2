create procedure dbo.sp_DeleteEmployeesRoles
    @RoleId int
as
begin
    set nocount on;

    delete from dbo.EmployeesRoles
    where RoleId = @RoleId;
end
go
