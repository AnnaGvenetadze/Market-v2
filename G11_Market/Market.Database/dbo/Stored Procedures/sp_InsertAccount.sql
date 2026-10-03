CREATE   PROCEDURE dbo.sp_InsertAccount
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(255),
    @IsActive BIT = 1,
    @FailedLoginAttempts INT = 0,
    @LockoutEndUtc DATETIME2 = NULL,
    @LastLoginAtUtc DATETIME2 = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF NULLIF(LTRIM(RTRIM(@Username)), '') IS NULL THROW 50000, 'Username is required.', 1;
    IF NULLIF(LTRIM(RTRIM(@PasswordHash)), '') IS NULL THROW 50000, 'PasswordHash is required.', 1;
    IF @IsActive IS NULL THROW 50000, 'IsActive is required.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Accounts WHERE Username = @Username) THROW 50000, 'Username already exists.', 1;

    INSERT INTO dbo.Accounts (Username, PasswordHash, IsActive, FailedLoginAttempts, LockoutEndUtc, LastLoginAtUtc, IsDeleted, CreateDate)
    VALUES (LTRIM(RTRIM(@Username)), @PasswordHash, @IsActive, COALESCE(@FailedLoginAttempts, 0), @LockoutEndUtc, @LastLoginAtUtc, 0, GETUTCDATE());

    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;