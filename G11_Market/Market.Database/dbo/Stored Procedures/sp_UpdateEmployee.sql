CREATE   PROCEDURE dbo.sp_UpdateEmployee
    @Id int,
    @AccountId int,
    @ManagerEmployeeId int,
    @FirstName nvarchar(50),
    @LastName nvarchar(50),
    @PhoneNumber nvarchar(20),
    @Email nvarchar(50),
    @EmployeeCode nvarchar(50),
    @HireDate date
as 
begin 
    set nocount on;
    set @FirstName = nullif(LTRIM(RTRIM(@FirstName)), '');
    set @LastName = nullif(LTRIM(RTRIM(@LastName)), '');
    set @PhoneNumber = nullif(LTRIM(RTRIM(@PhoneNumber)), '');
    set @Email = nullif(LTRIM(RTRIM(@Email)), '');
    set @EmployeeCode = nullif(LTRIM(RTRIM(@EmployeeCode)), '');

    IF @Email IS NULL THROW 50000, 'Email is required.', 1;
    IF @PhoneNumber IS NULL THROW 50000, 'PhoneNumber is required.', 1;
    IF @ManagerEmployeeId = @Id THROW 50000, 'Employee cannot manage themselves.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Employees WHERE AccountId = @AccountId AND Id <> @Id) THROW 50000, 'Account is already assigned to an employee.', 1;
    if @Id is null throw 50035, 'Employee ID is required for updates.', 1;
    if @AccountId is null throw 50013, 'AccountId is required.', 1;
    if @FirstName is null throw 50010, 'FirstName cannot be empty.', 1;
    if @LastName is null throw 50011, 'LastName cannot be empty.', 1;
    if @EmployeeCode is null throw 50023, 'EmployeeCode cannot be empty.', 1;
    if @HireDate is null throw 50024, 'HireDate is required.', 1;
    if @Email is not null and @Email not like '%_@_%._%' throw 50015, 'Invalid email format.', 1;
    if @PhoneNumber is not null and @PhoneNumber like '%[^0-9+ -]%' throw 50020, 'PhoneNumber contains invalid characters.', 1;
    if not exists (select 1 from Employees where Id = @Id and IsDeleted = 0) throw 50036, 'Employee not found or has been deleted.', 1;
    if not exists (select 1 from Accounts where Id = @AccountId and IsDeleted = 0) throw 50016, 'Invalid AccountId.', 1;
    if @ManagerEmployeeId is not null and not exists (select 1 from Employees where Id = @ManagerEmployeeId and IsDeleted = 0) throw 50025, 'Invalid ManagerEmployeeId.', 1;
    if exists (select 1 from Employees where EmployeeCode = @EmployeeCode and Id <> @Id) throw 50026, 'EmployeeCode already exists.', 1;
    if @Email is not null and exists (select 1 from Employees where Email = @Email and Id <> @Id and IsDeleted = 0) throw 50021, 'Email already exists.', 1;
    if @PhoneNumber is not null and exists (select 1 from Employees where PhoneNumber = @PhoneNumber and Id <> @Id and IsDeleted = 0) throw 50022, 'Phone number already exists.', 1;

    update Employees
    set AccountId = @AccountId,
        ManagerEmployeeId = @ManagerEmployeeId,
        FirstName = @FirstName,
        LastName = @LastName,
        PhoneNumber = @PhoneNumber,
        Email = @Email,
        EmployeeCode = @EmployeeCode,
        HireDate = @HireDate,
        UpdateDate = getdate()
    where Id = @Id and IsDeleted = 0;
      
    return 0;
end