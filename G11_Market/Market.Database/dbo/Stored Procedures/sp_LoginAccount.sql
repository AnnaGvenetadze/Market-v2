CREATE   PROCEDURE dbo.sp_LoginAccount
    @Username nvarchar(50)
as
begin
    set nocount on;
    set @Username = LTRIM(RTRIM(@Username));
    if @Username is null or @Username = '' throw 50050, 'Username is required.', 1;
    select * from Accounts where Username = @Username;
    return 0;
end