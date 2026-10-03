CREATE   PROCEDURE dbo.sp_UpdateAccountLoginAttempts
    @Id INT,
    @FailedLoginAttempts INT,
    @LockoutEndUtc DATETIME2 = NULL,
    @LastLoginAtUtc DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @FailedLoginAttempts IS NULL OR @FailedLoginAttempts < 0 THROW 50000, 'Invalid failed login count.', 1;

    UPDATE dbo.Accounts 
    SET FailedLoginAttempts = @FailedLoginAttempts, 
        LockoutEndUtc = @LockoutEndUtc,
        LastLoginAtUtc = COALESCE(@LastLoginAtUtc, LastLoginAtUtc), 
        UpdateDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0 THROW 50000, 'Account not found or deleted.', 1;
END;
GO