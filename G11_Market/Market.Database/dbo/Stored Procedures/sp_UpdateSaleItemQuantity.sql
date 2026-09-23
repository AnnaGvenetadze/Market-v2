CREATE PROCEDURE dbo.sp_UpdateSaleItemQuantity
    @Id INT,
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Quantity <= 0
        THROW 50039, 'Quantity must be greater than zero.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.SaleItems
        WHERE Id = @Id
    )
        THROW 50040, 'Sale item not found.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.SaleItems si
        INNER JOIN dbo.Sales s
            ON s.Id = si.SaleId
        WHERE si.Id = @Id
          AND s.Status = 0
    )
        THROW 50041, 'Sale item can be changed only while the sale is draft.', 1;

    UPDATE dbo.SaleItems
    SET Quantity = @Quantity
    WHERE Id = @Id;
END;
GO