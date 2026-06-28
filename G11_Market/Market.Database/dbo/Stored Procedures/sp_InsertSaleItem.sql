CREATE PROCEDURE dbo.sp_InsertSaleItem
    @SaleId INT,
    @ProductId INT,
    @Quantity INT,
    @UnitPrice MONEY,
    @DiscountAmount MONEY = 0,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;


    IF @SaleId IS NULL THROW 50030, 'SaleId is required.', 1;
    IF @ProductId IS NULL THROW 50031, 'ProductId is required.', 1;
    IF @Quantity IS NULL OR @Quantity <= 0 THROW 50032, 'Quantity must be greater than zero.', 1;
    IF @UnitPrice IS NULL OR @UnitPrice < 0 THROW 50033, 'UnitPrice cannot be negative.', 1;
    IF @DiscountAmount < 0 THROW 50034, 'DiscountAmount cannot be negative.', 1;


    IF NOT EXISTS (SELECT 1 FROM dbo.Sales WHERE Id = @SaleId) 
        THROW 50035, 'Parent Sale not found.', 1;
        
    IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Id = @ProductId) 
        THROW 50036, 'Product not found.', 1;


    IF EXISTS (SELECT 1 FROM dbo.SaleItems WHERE SaleId = @SaleId AND ProductId = @ProductId)
        THROW 50037, 'This product already exists in the sale.', 1;


    INSERT INTO dbo.SaleItems (SaleId, ProductId, Quantity, UnitPrice, DiscountAmount)
    VALUES (@SaleId, @ProductId, @Quantity, @UnitPrice, @DiscountAmount);


    SET @Id = SCOPE_IDENTITY();

    RETURN 0;
END
GO