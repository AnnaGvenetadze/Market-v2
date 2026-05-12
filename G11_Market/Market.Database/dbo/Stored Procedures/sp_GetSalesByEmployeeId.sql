create procedure dbo.sp_GetSalesByEmployeeId
	@EmployeeId int

as
begin
	set nocount on;

	if not exists 
		(select 1 from dbo.Employees where Id = @EmployeeId and IsDeleted = 0)
	
		begin
			;throw 50001, N'The specified employee does not exist or has been deleted.', 1;
		end

	select
        s.Id,
        s.CreatedEmployeeId,
        e.FirstName,
        e.LastName,
        s.CancelledByEmployeeId,
        s.Status,
        s.CreatedAt,
        s.CancelledAt,
        s.CancelReason

    from dbo.Sales s
    inner join dbo.Employees e on s.CreatedEmployeeId = e.Id
    where s.CreatedEmployeeId = @EmployeeId
    order by s.CreatedAt desc;
end
go