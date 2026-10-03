CREATE   PROCEDURE dbo.sp_InsertStockMovement
    @ProductId INT,
    @MovementType TINYINT,
    @QuantityChange INT,
    @QuantityBefore INT,
    @SaleItemId INT = NULL,
    @ChangedByEmployeeId INT,
    @Reason NVARCHAR(200) = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.StockMovements (ProductId, MovementType, QuantityChange, QuantityBefore, SaleItemId, ChangedByEmployeeId, Reason, CreatedDate)
    VALUES (@ProductId, @MovementType, @QuantityChange, @QuantityBefore, @SaleItemId, @ChangedByEmployeeId, @Reason, GETUTCDATE());

    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;