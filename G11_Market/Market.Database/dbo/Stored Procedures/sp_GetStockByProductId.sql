CREATE   PROCEDURE dbo.sp_GetStockByProductId
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
        @ProductId AS ProductId,
        COALESCE(SUM(QuantityChange), 0) AS CurrentStock
    FROM dbo.StockMovements
    WHERE ProductId = @ProductId;
END;