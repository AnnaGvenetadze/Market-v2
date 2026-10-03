CREATE   PROCEDURE dbo.sp_GetStockMovementById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ProductId, MovementType, QuantityChange, QuantityBefore, SaleItemId, ChangedByEmployeeId, Reason, CreatedDate
    FROM dbo.StockMovements
    WHERE Id = @Id;
END;