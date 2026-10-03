CREATE   PROCEDURE dbo.sp_ResetAccountPassword
    @AccountId INT,
    @NewPasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewPasswordHash = LTRIM(RTRIM(@NewPasswordHash));
    IF @NewPasswordHash IS NULL OR @NewPasswordHash = '' THROW 50041, 'Password hash is required.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE Id = @AccountId AND IsDeleted = 0) THROW 50043, 'Account not found.', 1;

    UPDATE dbo.Accounts
    SET PasswordHash = @NewPasswordHash, UpdateDate = GETUTCDATE() 
    WHERE Id = @AccountId;
END;