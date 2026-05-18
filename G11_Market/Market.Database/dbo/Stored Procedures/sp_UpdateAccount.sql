create procedure dbo.sp_UpdateAccount
    @Id int,
    @Username nvarchar(50),
    @PasswordHash nvarchar(255),
    @Email nvarchar(255),
    @FirstName nvarchar(50),
    @LastName nvarchar(50),
    @AccountType tinyint
as 
begin 
    set nocount on;
    set @Username = nullif(trim(@Username), '');
    set @PasswordHash = nullif(trim(@PasswordHash), '');
    set @Email = nullif(trim(@Email), '');
    set @FirstName = nullif(trim(@FirstName), '');
    set @LastName = nullif(trim(@LastName), '');

    if @Id is null throw 50030, 'Account ID is required for updates.', 1;
    if @Username is null throw 50001, 'Username cannot be empty.', 1;
    if @PasswordHash is null throw 50002, 'PasswordHash cannot be empty.', 1;
    if @Email is null throw 50003, 'Email cannot be empty.', 1;
    if @Email not like '%_@_%._%' throw 50004, 'Invalid email format.', 1;
    if @FirstName is null throw 50007, 'FirstName cannot be empty.', 1;
    if @LastName is null throw 50008, 'LastName cannot be empty.', 1;
    if @AccountType is null throw 50009, 'AccountType is required.', 1;
    if @AccountType not in (1, 2, 3) throw 50000, 'Invalid account type. Use 1 for Employee, 2 for Individual Client, 3 for Corporate Client', 1;
    if not exists (select 1 from Accounts where Id = @Id and IsDeleted = 0)throw 50031, 'Account not found or has been deleted.', 1;
    if exists (select 1 from Accounts where Username = @Username and Id <> @Id) throw 50005, 'Username already exists.', 1;         
    if exists (select 1 from Accounts where Email = @Email and Id <> @Id) throw 50006, 'Email already exists.', 1;

    update Accounts
    set Username = @Username,
            PasswordHash = @PasswordHash,
            Email = @Email,
            FirstName = @FirstName,
            LastName = @LastName,
            AccountType = @AccountType,
            UpdateDate = getdate()
     where Id = @Id;
      
    return 0;
end