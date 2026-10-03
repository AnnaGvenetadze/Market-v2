CREATE   PROCEDURE dbo.sp_UpdateSaleItem
    @Id INT,
    @SaleId INT = NULL,
    @ProductId INT = NULL,
    @Quantity INT,
    @UnitPrice MONEY = NULL,
    @DiscountAmount MONEY = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Quantity IS NULL OR @Quantity <= 0 THROW 50039, 'Quantity must be greater than zero.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.SaleItems WHERE Id = @Id) THROW 50040, 'Sale item not found.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.SaleItems si INNER JOIN dbo.Sales s ON s.Id = si.SaleId WHERE si.Id = @Id AND s.Status = 0)
        THROW 50041, 'Sale item can be changed only while the sale is draft.', 1;

    UPDATE dbo.SaleItems
    SET Quantity = @Quantity,
        UnitPrice = COALESCE(@UnitPrice, UnitPrice),
        DiscountAmount = COALESCE(@DiscountAmount, DiscountAmount)
    WHERE Id = @Id;
END;