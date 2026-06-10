CREATE PROCEDURE dbo.sp_UpdateSale
    @Id                     INT,
    @CancelledByEmployeeId  INT = NULL,
    @Status                 TINYINT,
    @CancelledDate         DATETIME = NULL,
    @CancelReason          NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Sales WHERE Id = @Id)
    BEGIN
        RAISERROR('Sale not found.', 16, 1);
        RETURN;
    END

    IF @Status NOT IN (0, 1, 2)
    BEGIN
        RAISERROR('Invalid status.', 16, 1);
        RETURN;
    END

    UPDATE dbo.Sales
    SET
        CancelledByEmployeeId = @CancelledByEmployeeId,
        Status = @Status,
        CancelledDate = @CancelledDate,
        CancelReason = @CancelReason
    WHERE Id = @Id;
END
GO