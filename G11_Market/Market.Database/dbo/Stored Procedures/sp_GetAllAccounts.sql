create procedure dbo.sp_GetAllAccounts
as 
begin 
    set nocount on;
    select * from Accounts where IsDeleted = 0;
        
    return 0;
end