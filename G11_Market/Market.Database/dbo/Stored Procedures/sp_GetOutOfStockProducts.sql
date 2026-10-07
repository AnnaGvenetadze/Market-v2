CREATE PROCEDURE dbo.sp_GetOutOfStockProducts
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Id AS ProductId,
        p.ProductName,
        COALESCE(
            lastMovement.QuantityBefore + lastMovement.QuantityChange,
            0
        ) AS CurrentQuantity
    FROM dbo.Products AS p
    OUTER APPLY
    (
        SELECT TOP (1)
            sm.QuantityBefore,
            sm.QuantityChange
        FROM dbo.StockMovements AS sm
        WHERE sm.ProductId = p.Id
        ORDER BY sm.Id DESC
    ) AS lastMovement
    WHERE p.IsDeleted = 0
      AND COALESCE(
            lastMovement.QuantityBefore + lastMovement.QuantityChange,
            0
          ) <= 0
    ORDER BY p.ProductName;
END;