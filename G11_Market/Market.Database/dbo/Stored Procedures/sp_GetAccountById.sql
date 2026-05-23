create procedure dbo.sp_GetAccountById
    @Id int
as 
begin 
    set nocount on;
    if @Id is null throw 50030, 'Account ID is required.', 1;
    if not exists (select 1 from Accounts where Id = @Id and IsDeleted = 0) throw 50031, 'Account not found or has been deleted.', 1;

    select * from Accounts where Id = @Id and IsDeleted = 0;
      
    return 0;
end