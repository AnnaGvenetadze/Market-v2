create procedure sp_DeleteEmployee
	@EmployeeID int
as
begin
	set nocount on;
	update Employees
	set
		IsDeleted = 1,
		UpdateDate = getdate() 
	where Id = @EmployeeID
		and IsDeleted = 0;
	if @@ROWCOUNT = 0
	begin
        ;throw 50001, N'Employee does not exist or has already been deleted', 1;
    end

end
go 
