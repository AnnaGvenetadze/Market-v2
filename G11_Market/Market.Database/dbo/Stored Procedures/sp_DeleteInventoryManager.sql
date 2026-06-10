create procedure dbo.sp_DeleteInventoryManager
    @Id int
as
begin
    set nocount on;

    if @Id is null
        throw 50116, 'Inventory manager ID is required for delete.', 1;

    if not exists
    (
        select 1
        from dbo.InventoryManagerDetails
        where Id = @Id
          and IsDeleted = 0
    )
        throw 50117, 'Inventory manager details were not found or have already been deleted.', 1;

    update dbo.InventoryManagerDetails
    set IsDeleted = 1,
        UpdateDate = getdate()
    where Id = @Id
      and IsDeleted = 0;

    return 0;
end;
go