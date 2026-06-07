create procedure dbo.sp_InsertInventoryManager
    @Id int,
    @StockAdjustmentLimit decimal(18,2),
    @CanApproveStockCorrection bit,
    @CanApproveNegativeStock bit
as
begin
    set nocount on;

    if @Id is null throw 50001, 'Employee ID is required.', 1;
    if @StockAdjustmentLimit < 0 throw 50002, 'StockAdjustmentLimit cannot be negative.', 1;

    if not exists (
        select 1
        from Employees
        where Id = @Id and IsDeleted = 0
    )
        throw 50003, 'Employee was not found.', 1;

    if exists (
        select 1
        from InventoryManagerDetails
        where Id = @Id
    )
        throw 50004, 'Inventory manager details already exist for this employee.', 1;

    insert into InventoryManagerDetails
    (
        Id,
        StockAdjustmentLimit,
        CanApproveStockCorrection,
        CanApproveNegativeStock
    )
    values
    (
        @Id,
        @StockAdjustmentLimit,
        @CanApproveStockCorrection,
        @CanApproveNegativeStock
    );

    return 0;
end