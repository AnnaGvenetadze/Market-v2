CREATE PROCEDURE dbo.sp_RefreshToken
    @Id INT,
    @Token VARBINARY(32),
    @TokenExpiration DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id <= 0
    BEGIN
        RAISERROR('Invalid AccountId.', 16, 1);
        RETURN;
    END;

    IF @Token IS NULL OR DATALENGTH(@Token) <> 32
    BEGIN
        RAISERROR('Token hash must contain exactly 32 bytes.', 16, 1);
        RETURN;
    END;

    IF @TokenExpiration IS NULL
    BEGIN
        RAISERROR('TokenExpiration cannot be null.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Accounts
        WHERE Id = @Id
          AND IsDeleted = 0
          AND IsActive = 1
    )
    BEGIN
        RAISERROR('Active account was not found.', 16, 1);
        RETURN;
    END;

    UPDATE dbo.Accounts
    SET
        Token = @Token,
        TokenExpiration = @TokenExpiration,
        UpdateDate = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0
      AND IsActive = 1;
END; 