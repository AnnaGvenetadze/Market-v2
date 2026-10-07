CREATE PROCEDURE dbo.sp_CancelSale
    @Id INT,
    @EmployeeId INT,
    @CancelReason NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @Status TINYINT;

        SELECT @Status = Status
        FROM dbo.Sales WITH (UPDLOCK, HOLDLOCK)
        WHERE Id = @Id;

        IF @Status IS NULL
            THROW 50116, 'Sale not found.', 1;

        IF @Status = 1
            THROW 50117, 'Completed sale cannot be cancelled.', 1;

        IF @Status = 2
            THROW 50118, 'Sale is already cancelled.', 1;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.Employees
            WHERE Id = @EmployeeId
              AND IsDeleted = 0
        )
            THROW 50119, 'Employee not found.', 1;

        IF @CancelReason IS NULL
           OR LEN(LTRIM(RTRIM(@CancelReason))) = 0
            THROW 50120, 'Cancel reason is required.', 1;

        UPDATE dbo.Sales
        SET Status = 2,
            CancelledByEmployeeId = @EmployeeId,
            CancelledDate = GETUTCDATE(),
            CancelReason = @CancelReason
        WHERE Id = @Id;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;