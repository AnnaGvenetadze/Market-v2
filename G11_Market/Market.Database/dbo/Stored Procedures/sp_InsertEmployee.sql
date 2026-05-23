create procedure dbo.sp_InsertEmployee
    @AccountId int,
    @FirstName nvarchar(50),
    @LastName nvarchar(50),
    @EmployeeCode nvarchar(50),
    @HireDate date,
    @PhoneNumber varchar(20), 
    @ContactEmail nvarchar(255),
    @ManagerEmployeeId int,
    @Id int output
as
begin
    set nocount on;
    set @FirstName = nullif(trim(@FirstName), '');
    set @LastName = nullif(trim(@LastName), '');
    set @EmployeeCode = nullif(trim(@EmployeeCode), '');
    set @ContactEmail = nullif(trim(@ContactEmail), '');
    set @PhoneNumber = nullif(trim(@PhoneNumber), '');

    if @FirstName is null throw 50024, 'FirstName cannot be empty.', 1;
    if @LastName is null throw 50025, 'LastName cannot be empty.', 1;
    if @EmployeeCode is null throw 50026, 'EmployeeCode cannot be empty.', 1;
    if @HireDate is null throw 50027, 'HireDate is required.', 1;
    if @AccountId is null throw 50028, 'AccountId is required.', 1;
    if @HireDate > getdate() throw 50019, 'HireDate cannot be in the future.', 1;
    if @ContactEmail is not null and @ContactEmail not like '%_@_%._%' throw 50022, 'Invalid ContactEmail format.', 1;
    if @ManagerEmployeeId is not null and not exists (select 1 from Employees where Id = @ManagerEmployeeId) throw 50018, 'Invalid ManagerEmployeeId.', 1;
    if exists (select 1 from Employees where EmployeeCode = @EmployeeCode) throw 50020, 'EmployeeCode must be unique.', 1;
    if not exists (select 1 from Accounts where Id = @AccountId and AccountType = 1) throw 50021, 'AccountId must reference a valid Employee account.', 1;

    insert into Employees (AccountId, ManagerEmployeeId, FirstName, LastName, PhoneNumber, ContactEmail, EmployeeCode, HireDate)
    values (@AccountId, @ManagerEmployeeId, @FirstName, @LastName, @PhoneNumber, @ContactEmail, @EmployeeCode, @HireDate);

    set @Id = scope_identity();

    return 0;
end