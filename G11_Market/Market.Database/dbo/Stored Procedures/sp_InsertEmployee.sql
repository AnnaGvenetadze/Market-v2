create procedure dbo.sp_InsertEmployee
	@AccountId int,
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@EmployeeCode nvarchar(50),
	@HireDate date,
	@PhoneNumber varchar(20) = null,
	@ContactEmail nvarchar(255) = null,
	@ManagerEmployeeId int = null
as
begin
	set nocount on;
	insert into Employees (AccountId, ManagerEmployeeId, FirstName, LastName, PhoneNumber, ContactEmail, EmployeeCode, HireDate)
	values (@AccountId, @ManagerEmployeeId, @FirstName, @LastName, @PhoneNumber, @ContactEmail, @EmployeeCode, @HireDate);

	return 0;
end
