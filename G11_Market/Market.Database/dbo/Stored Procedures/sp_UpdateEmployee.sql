create procedure dbo.sp_UpdateEmployee
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
    if @AccountId <= 0 throw 50070, 'Invalid AccountId.', 1;

    set @FirstName = trim(@FirstName);
    set @LastName = trim(@LastName);
    set @EmployeeCode = trim(@EmployeeCode);
    set @ContactEmail = trim(@ContactEmail);
    set @PhoneNumber = trim(@PhoneNumber);

    if @FirstName is null or @FirstName = '' throw 50071, 'FirstName is required.', 1;
    if @LastName is null or @LastName = '' throw 50072, 'LastName is required.', 1;
    if @EmployeeCode is null or @EmployeeCode = '' throw 50073, 'EmployeeCode is required.', 1;
    if @HireDate is null throw 50074, 'HireDate is required.', 1;
    if @HireDate > getdate() throw 50075, 'HireDate cannot be in the future.', 1;
    if @ContactEmail is not null and @ContactEmail not like '%_@_%._%'  throw 50076, 'Invalid email format.', 1;
    if not exists (select 1 from Employees where AccountId = @AccountId) throw 50078, 'Employee not found.', 1;
    if @ManagerEmployeeId is not null and not exists (select 1 from Employees where Id = @ManagerEmployeeId) throw 50080, 'Manager employee does not exist.', 1;
    update Employees
    set
        FirstName = @FirstName,
        LastName = @LastName,
        EmployeeCode = @EmployeeCode,
        HireDate = @HireDate,
        PhoneNumber = @PhoneNumber,
        ContactEmail = @ContactEmail,
        ManagerEmployeeId = @ManagerEmployeeId
    where AccountId = @AccountId;
    return 0;
end