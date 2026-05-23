create procedure dbo.sp_DeleteAccount
    @Id int
as 
begin 
    set nocount on;
    if @Id is null throw 50030, 'Account ID is required for deletion.', 1;
    if not exists (select 1 from Accounts where Id = @Id and IsDeleted = 0) throw 50031, 'Account not found or has already been deleted.', 1;

    update Accounts
    set IsDeleted = 1,
        UpdateDate = getdate()
    where Id = @Id;

    return 0;
end