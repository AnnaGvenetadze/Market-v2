create procedure dbo.sp_GetAccountByUsername
    @Username nvarchar(50)
as
begin
    set nocount on;
    set @Username = trim(@Username);
    if @Username is null or @Username = '' throw 50030, 'Username is required.', 1;
    select * from Accounts where Username = @Username and IsDeleted = 0;
    return 0;
end