CREATE   PROCEDURE dbo.sp_GetAllSaleItems
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, SaleId, ProductId, Quantity, UnitPrice, DiscountAmount
    FROM dbo.SaleItems;
END;