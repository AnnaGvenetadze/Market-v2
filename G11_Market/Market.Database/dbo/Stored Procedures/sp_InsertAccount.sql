
CREATE   PROCEDURE dbo.sp_InsertAccount
    @Id INT = NULL OUTPUT,
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(255),
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF NULLIF(LTRIM(RTRIM(@Username)), '') IS NULL THROW 50000, 'Username is required.', 1;
    IF NULLIF(LTRIM(RTRIM(@PasswordHash)), '') IS NULL THROW 50000, 'PasswordHash is required.', 1;
    IF @IsActive IS NULL THROW 50000, 'IsActive is required.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Accounts WHERE Username = @Username) THROW 50000, 'Username already exists.', 1;
    INSERT dbo.Accounts (Username, PasswordHash, IsActive) VALUES (LTRIM(RTRIM(@Username)), @PasswordHash, @IsActive);
    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;