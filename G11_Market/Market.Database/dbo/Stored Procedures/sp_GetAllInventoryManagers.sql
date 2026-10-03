CREATE   PROCEDURE dbo.sp_GetAllInventoryManagers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, StockAdjustmentLimit, CanApproveStockCorrection, CanApproveNegativeStock, IsDeleted, CreateDate, UpdateDate
    FROM dbo.InventoryManagerDetails
    WHERE IsDeleted = 0
    ORDER BY Id;
END;
go