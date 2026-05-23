create procedure dbo.sp_GetEmployeeByAcountId
	@AccountId int

as 
begin
	set nocount on;

	select 
	emp.Id as EmployeeId,
	emp.ContactEmail,
	emp.FirstName,
	emp.LastName,
	emp.ContactEmail,
	emp.PhoneNumber,
	emp.EmployeeCode
	from dbo.Employees emp
	inner join dbo.Accounts acc
	on emp.AccountId = acc.Id
	where acc.Id = @AccountId and emp.IsDeleted = 0 and acc.IsDeleted = 0;
end
go