CREATE PROCEDURE dbo.sp_GetSaleItemById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        SaleId,
        ProductId,
        Quantity,
        UnitPrice,
        DiscountAmount
    FROM dbo.SaleItems
    WHERE Id = @Id;
END;
GO