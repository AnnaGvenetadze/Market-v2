CREATE PROCEDURE dbo.sp_InsertSaleItem
    @SaleId INT,
    @ProductId INT,
    @Quantity INT,
    @UnitPrice MONEY,
    @DiscountAmount MONEY,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Quantity <= 0
        THROW 50032, 'Quantity must be greater than zero.', 1;

    IF @UnitPrice < 0
        THROW 50033, 'UnitPrice cannot be negative.', 1;

    IF @DiscountAmount < 0
        THROW 50034, 'DiscountAmount cannot be negative.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Sales
        WHERE Id = @SaleId
    )
        THROW 50035, 'Sale not found.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Sales
        WHERE Id = @SaleId
          AND Status = 0
    )
        THROW 50038, 'Sale items can be added only to a draft sale.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Products
        WHERE Id = @ProductId
          AND IsDeleted = 0
    )
        THROW 50036, 'Product not found or inactive.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.SaleItems
        WHERE SaleId = @SaleId
          AND ProductId = @ProductId
    )
        THROW 50037, 'This product already exists in the sale.', 1;

    INSERT INTO dbo.SaleItems
    (
        SaleId,
        ProductId,
        Quantity,
        UnitPrice,
        DiscountAmount
    )
    VALUES
    (
        @SaleId,
        @ProductId,
        @Quantity,
        @UnitPrice,
        @DiscountAmount
    );

    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;
GO