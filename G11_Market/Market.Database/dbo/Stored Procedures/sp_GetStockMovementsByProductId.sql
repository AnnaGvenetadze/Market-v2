CREATE PROCEDURE dbo.sp_GetStockMovementsByProductId
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Products
        WHERE Id = @ProductId
    )
        THROW 50200, 'Product not found.', 1;

    SELECT
        Id,
        ProductId,
        MovementType,
        QuantityChange,
        QuantityBefore,
        SaleItemId,
        ChangedByEmployeeId,
        Reason,
        CreatedDate
    FROM dbo.StockMovements
    WHERE ProductId = @ProductId
    ORDER BY CreatedDate, Id;
END;
GO