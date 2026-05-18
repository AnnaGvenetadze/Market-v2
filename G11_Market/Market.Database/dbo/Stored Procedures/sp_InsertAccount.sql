create procedure dbo.sp_InsertAccount
    @Username nvarchar(50),
    @PasswordHash nvarchar(255),
    @Email nvarchar(255),
    @FirstName nvarchar(50),
    @LastName nvarchar(50),
    @AccountType tinyint, -- 1=employee, 2=individual client, 3=corporate client
    @Id int output
as 
begin 
    set nocount on;

    set @Username = nullif(trim(@Username), '');
    set @PasswordHash = nullif(trim(@PasswordHash), '');
    set @Email = nullif(trim(@Email), '');
    set @FirstName = nullif(trim(@FirstName), '');
    set @LastName = nullif(trim(@LastName), '');

    if @Username is null throw 50001, 'Username cannot be empty.', 1;
    if @PasswordHash is null throw 50002, 'PasswordHash cannot be empty.', 1;
    if @Email is null throw 50003, 'Email cannot be empty.', 1;
    if @Email not like '%_@_%._%' throw 50004, 'Invalid email format.', 1;
    if @FirstName is null throw 50007, 'FirstName cannot be empty.', 1;
    if @LastName is null throw 50008, 'LastName cannot be empty.', 1;
    if @AccountType is null throw 50009, 'AccountType is required.', 1;
    if @AccountType not in (1, 2, 3) throw 50000, 'Invalid account type. Use 1 for Employee, 2 for Individual Client, 3 for Corporate Client', 1;
    if exists (select 1 from Accounts where Username = @Username) throw 50005, 'Username already exists.', 1;    
    if exists (select 1 from Accounts where Email = @Email) throw 50006, 'Email already exists.', 1;

    insert into Accounts (Username, PasswordHash, Email, FirstName, LastName, AccountType, IsDeleted, CreateDate)
    values (@Username, @PasswordHash, @Email, @FirstName, @LastName, @AccountType, 0, getdate());

    set @Id = scope_identity();

    return 0;
end