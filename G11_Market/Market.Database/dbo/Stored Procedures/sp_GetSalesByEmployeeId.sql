create procedure dbo.sp_GetSalesByEmployeeId
	@EmployeeId int

as
begin
	set nocount on;

	select
        s.Id,
        s.CreatedEmployeeId,
        e.FirstName,
        e.LastName,
        s.CancelledByEmployeeId,
        s.Status,
        s.CreatedDate,
        s.CancelledDate,
        s.CancelReason

    from dbo.Sales s
    inner join dbo.Employees e on s.CreatedEmployeeId = e.Id
    where s.CreatedEmployeeId = @EmployeeId
    order by s.CreatedDate;
end
go