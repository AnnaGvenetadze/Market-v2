CREATE PROCEDURE [dbo].[sp_UpdateAccountLoginAttempts]
    @Id INT,
    @FailedLoginAttempts INT,
    @LockoutTime DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Accounts
    SET 
        FailedLoginAttempts = @FailedLoginAttempts,
        LockoutTime = @LockoutTime,
        UpdateDate = GETUTCDATE()
    WHERE 
        Id = @Id;
END;
GO