CREATE   PROCEDURE dbo.sp_UpdateSale
    @Id INT,
    @CreatedEmployeeId INT = NULL,
    @Status TINYINT = NULL,
    @CancelledByEmployeeId INT = NULL,
    @CancelledDate DATETIME = NULL,
    @CancelReason NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Sales
    SET CreatedEmployeeId = COALESCE(@CreatedEmployeeId, CreatedEmployeeId),
        Status = COALESCE(@Status, Status),
        CancelledByEmployeeId = COALESCE(@CancelledByEmployeeId, CancelledByEmployeeId),
        CancelledDate = COALESCE(@CancelledDate, CancelledDate),
        CancelReason = COALESCE(@CancelReason, CancelReason)
    WHERE Id = @Id;

    IF @@ROWCOUNT = 0 THROW 50116, 'Sale not found.', 1;
END;