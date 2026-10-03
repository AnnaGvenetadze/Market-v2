CREATE   PROCEDURE dbo.sp_GetAllStockMovements
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ProductId, MovementType, QuantityChange, QuantityBefore, SaleItemId, ChangedByEmployeeId, Reason, CreatedDate
    FROM dbo.StockMovements;
END;