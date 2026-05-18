create procedure dbo.sp_GetClientById
    @Id int
as 
begin 
    set nocount on;

    if @Id is null throw 50033, 'Client ID is required.', 1;
    if not exists (select 1 from Clients where Id = @Id and IsDeleted = 0) throw 50034, 'Client not found or has been deleted.', 1;

    select * from Clients where Id = @Id and IsDeleted = 0;
        
    return 0;
end