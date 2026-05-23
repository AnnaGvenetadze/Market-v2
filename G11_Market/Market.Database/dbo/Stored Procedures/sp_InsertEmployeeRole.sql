create procedure sp_InsertEmployeeRole
    @EmployeeId int,
    @RoleId int
as
begin
    set nocount on;
    if not exists (
        select 1 from dbo.EmployeesRoles
        where EmployeeId = @EmployeeId
          and RoleId = @RoleId
    )
    begin
        insert into dbo.EmployeesRoles (EmployeeId, RoleId)
        values (@EmployeeId, @RoleId);
    end
end
go