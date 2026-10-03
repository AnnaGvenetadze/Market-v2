CREATE   PROCEDURE dbo.sp_DeleteStockMovement
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.StockMovements WHERE Id = @Id;
END;