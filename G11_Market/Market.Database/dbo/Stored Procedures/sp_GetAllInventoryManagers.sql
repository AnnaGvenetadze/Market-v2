create procedure dbo.sp_GetAllInventoryManagers
as
begin
    set nocount on;

    select
        Id,
        StockAdjustmentLimit,
        CanApproveStockCorrection,
        CanApproveNegativeStock,
        IsDeleted,
        CreateDate,
        UpdateDate
    from dbo.InventoryManagerDetails
    where IsDeleted = 0
    order by Id;

    return 0;
end;
go