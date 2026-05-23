create procedure dbo.sp_DeleteClient
    @Id int

as 
begin 
    set nocount on;
    if @Id is null throw 50033, 'Client ID is required for deletion.', 1;
    if not exists (select 1 from Clients where Id = @Id and IsDeleted = 0) throw 50034, 'Client not found or has already been deleted.', 1;

    update dbo.Clients
    set
        IsDeleted = 1,
        UpdateDate = getdate()
    where Id = @Id
      and IsDeleted = 0;

    if @@rowcount = 0
    begin
        ;throw 50001, N'Client does not exist or has already been deleted.', 1;
    end
        
    return 0;
end
go