CREATE PROCEDURE dbo.sp_GetSaleById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        CreatedEmployeeId,
        CancelledByEmployeeId,
        Status,
        CreatedDate,
        CancelledDate,
        CancelReason
    FROM dbo.Sales
    WHERE Id = @Id;
END
GO