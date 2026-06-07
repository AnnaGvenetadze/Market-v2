create procedure dbo.sp_UpdateInventoryManager
    @Id int,
    @StockAdjustmentLimit decimal(18,2),
    @CanApproveStockCorrection bit,
    @CanApproveNegativeStock bit
as
begin
    set nocount on;

    if @Id is null
        throw 50112, 'Inventory manager ID is required for update.', 1;

    if @StockAdjustmentLimit is null
        throw 50113, 'StockAdjustmentLimit is required.', 1;

    if @StockAdjustmentLimit < 0
        throw 50114, 'StockAdjustmentLimit cannot be negative.', 1;

    if not exists
    (
        select 1
        from dbo.InventoryManagerDetails
        where Id = @Id
          and IsDeleted = 0
    )
        throw 50115, 'Inventory manager details were not found or have been deleted.', 1;

    update dbo.InventoryManagerDetails
    set StockAdjustmentLimit = @StockAdjustmentLimit,
        CanApproveStockCorrection = @CanApproveStockCorrection,
        CanApproveNegativeStock = @CanApproveNegativeStock,
        UpdateDate = getdate()
    where Id = @Id
      and IsDeleted = 0;

    return 0;
end;
go