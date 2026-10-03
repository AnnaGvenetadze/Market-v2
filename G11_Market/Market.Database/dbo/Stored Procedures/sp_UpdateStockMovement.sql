CREATE   PROCEDURE dbo.sp_UpdateStockMovement
    @Id INT,
    @ProductId INT,
    @MovementType TINYINT,
    @QuantityChange INT,
    @QuantityBefore INT,
    @SaleItemId INT = NULL,
    @ChangedByEmployeeId INT,
    @Reason NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.StockMovements
    SET ProductId = @ProductId,
        MovementType = @MovementType,
        QuantityChange = @QuantityChange,
        QuantityBefore = @QuantityBefore,
        SaleItemId = @SaleItemId,
        ChangedByEmployeeId = @ChangedByEmployeeId,
        Reason = @Reason
    WHERE Id = @Id;
END;