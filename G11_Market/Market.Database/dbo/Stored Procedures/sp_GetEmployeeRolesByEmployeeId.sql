create procedure sp_GetEmployeeRolesByEmployeeId
	@EmployeeId int
as
begin
		select 
	r.Id,
	r.Name,
	r.Description
	from Roles as r
	inner join EmployeesRoles as er	
		on r.Id = er.RoleId
	where er.EmployeeId = @EmployeeId
	  and r.IsDeleted = 0;
end
go
