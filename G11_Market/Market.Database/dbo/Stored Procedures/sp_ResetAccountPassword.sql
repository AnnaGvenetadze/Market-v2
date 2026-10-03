CREATE   PROCEDURE dbo.sp_ResetAccountPassword
    @AccountId int,
    @NewPasswordHash nvarchar(255)
as
begin
    set nocount on;
    set @NewPasswordHash = LTRIM(RTRIM(@NewPasswordHash));
    if @NewPasswordHash is null or @NewPasswordHash = '' throw 50041, 'Password hash is required.', 1;
    if not exists (select 1 from Accounts where Id = @AccountId and IsDeleted = 0) throw 50043, 'Account not found.', 1;
    update Accounts
    set PasswordHash = @NewPasswordHash, UpdateDate = GETDATE() where Id = @AccountId;
    return 0;
end