CREATE PROCEDURE dbo.sp_DeleteSaleItem
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

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
        THROW 50042, 'Sale item can be deleted only while the sale is draft.', 1;

    DELETE FROM dbo.SaleItems
    WHERE Id = @Id;
END;