
CREATE   PROCEDURE dbo.sp_GetOutOfStockProducts
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Id,
        p.CategoryId,
        p.ProductName,
        p.Price,
        COALESCE(SUM(sm.QuantityChange), 0) AS CurrentStock
    FROM dbo.Products p
    LEFT JOIN dbo.StockMovements sm
        ON sm.ProductId = p.Id
    WHERE p.IsDeleted = 0
    GROUP BY
        p.Id,
        p.CategoryId,
        p.ProductName,
        p.Price
    HAVING COALESCE(SUM(sm.QuantityChange), 0) <= 0
    ORDER BY p.ProductName;
END;