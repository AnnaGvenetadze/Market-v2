CREATE PROCEDURE dbo.sp_GetIncomeByDateRange
    @DateFrom DATETIME,
    @DateTo DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    IF @DateFrom > @DateTo
    BEGIN
        RAISERROR(
            N'DateFrom cannot be greater than DateTo.',
            16,
            1
        );
        RETURN;
    END;

    SELECT
        COALESCE(
            SUM(
                si.Quantity * si.UnitPrice
                - si.DiscountAmount
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