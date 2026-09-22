create procedure dbo.sp_GetInventoryManagerDetailById
    @Id int
as
begin
    set nocount on;

    if @Id is null
        throw 50110, 'Inventory manager ID is required.', 1;

    if not exists
    (
        select 1
        from dbo.InventoryManagerDetails
        where Id = @Id
          and IsDeleted = 0
    )
        throw 50111, 'Inventory manager details were not found or have been deleted.', 1;

    select
        Id,
        StockAdjustmentLimit,
        CanApproveStockCorrection,
        CanApproveNegativeStock,
        IsDeleted,
        CreateDate,
        UpdateDate
    from dbo.InventoryManagerDetails
    where Id = @Id
      and IsDeleted = 0;

    return 0;
end;
go