CREATE   PROCEDURE dbo.sp_UpdateAccount
    @Id INT,
    @Username NVARCHAR(50) = NULL,
    @PasswordHash NVARCHAR(255) = NULL,
    @IsActive BIT = NULL,
    @FailedLoginAttempts INT = NULL,
    @LockoutEndUtc DATETIME2 = NULL,
    @LastLoginAtUtc DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Username IS NOT NULL AND NULLIF(LTRIM(RTRIM(@Username)), '') IS NULL THROW 50000, 'Username cannot be empty.', 1;
    IF @PasswordHash IS NOT NULL AND NULLIF(LTRIM(RTRIM(@PasswordHash)), '') IS NULL THROW 50000, 'PasswordHash cannot be empty.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Accounts WHERE Username = @Username AND Id <> @Id) THROW 50000, 'Username already exists.', 1;

    UPDATE dbo.Accounts 
    SET Username = COALESCE(LTRIM(RTRIM(@Username)), Username),
        PasswordHash = COALESCE(@PasswordHash, PasswordHash),
        IsActive = COALESCE(@IsActive, IsActive),
        FailedLoginAttempts = COALESCE(@FailedLoginAttempts, FailedLoginAttempts),
        LockoutEndUtc = COALESCE(@LockoutEndUtc, LockoutEndUtc),
        LastLoginAtUtc = COALESCE(@LastLoginAtUtc, LastLoginAtUtc),
        UpdateDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0 THROW 50000, 'Account not found or deleted.', 1;
END;