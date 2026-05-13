create procedure dbo.sp_UpdateAccount
    @Id int,
    @Email nvarchar(255),
    @FirstName nvarchar(100),
    @LastName nvarchar(100)
as
begin
    set nocount on;
    set @Email = trim(@Email);
    set @FirstName = trim(@FirstName);
    set @LastName = trim(@LastName);

    if @Email is null or @Email = '' throw 50061, 'Email is required.', 1;
    if @Email not like '%_@_%._%' throw 50062, 'Invalid email format.', 1;
    if not exists (select 1 from Accounts where Id = @Id and IsDeleted = 0) throw 50063, 'Account not found.', 1;

    update Accounts
    set Email = @Email, FirstName = @FirstName,LastName = @LastName where Id = @Id;
    return 0;
end