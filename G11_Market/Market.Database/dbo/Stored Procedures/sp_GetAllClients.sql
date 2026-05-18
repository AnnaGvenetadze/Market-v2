create procedure dbo.sp_GetAllClients
as 
begin 
    set nocount on;
    select * from Clients where IsDeleted = 0;
        
    return 0;
end