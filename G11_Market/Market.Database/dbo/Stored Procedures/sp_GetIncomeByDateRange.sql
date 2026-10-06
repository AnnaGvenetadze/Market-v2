CREATE PROCEDURE dbo.sp_GetIncomeByDateRange
    @DateFrom DATETIME2,
    @DateTo DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    IF @DateFrom >= @DateTo
    BEGIN
        RAISERROR(
            N'DateFrom must be earlier than DateTo.',
            16,
            1
        );
        RETURN;
    END;

    SELECT
        COALESCE(
            SUM(
                si.Quantity * si.UnitPrice
                - ISNULL(si.DiscountAmount, 0)
            ),
            0
        )
    FROM dbo.Sales AS s
    INNER JOIN dbo.SaleItems AS si
        ON si.SaleId = s.Id
    WHERE s.Status = 1
      AND s.CreatedDate >= @DateFrom
      AND s.CreatedDate < @DateTo;
END;