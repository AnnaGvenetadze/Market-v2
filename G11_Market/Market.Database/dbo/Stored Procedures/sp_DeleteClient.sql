create procedure dbo.sp_DeleteClient
    @Id int

as
begin
    set nocount on;

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

end
go