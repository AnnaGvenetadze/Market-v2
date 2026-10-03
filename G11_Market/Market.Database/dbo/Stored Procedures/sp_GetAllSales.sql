CREATE   PROCEDURE dbo.sp_GetAllSales
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, CreatedEmployeeId, CancelledByEmployeeId, Status, CreatedDate, CancelledDate, CancelReason
    FROM dbo.Sales;
END;