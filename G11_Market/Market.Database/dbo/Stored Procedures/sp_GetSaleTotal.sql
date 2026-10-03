CREATE   PROCEDURE dbo.sp_GetSaleTotal
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.Id, COALESCE(SUM(CONVERT(DECIMAL(38,4), si.Quantity) * (si.UnitPrice - si.DiscountAmount)), 0) AS Total
    FROM dbo.Sales s LEFT JOIN dbo.SaleItems si ON si.SaleId = s.Id WHERE s.Id = @Id GROUP BY s.Id;
END;
go