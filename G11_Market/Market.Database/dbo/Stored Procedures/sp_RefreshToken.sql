CREATE PROCEDURE dbo.sp_RefreshToken
    @AccountId INT,
    @Token NVARCHAR(500),
    @TokenExpiration DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    IF @AccountId <= 0
    BEGIN
        RAISERROR('Invalid AccountId.', 16, 1);
        RETURN;
    END;

    IF @Token IS NULL OR LTRIM(RTRIM(@Token)) = ''
    BEGIN
        RAISERROR('Token cannot be null or empty.', 16, 1);
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
        WHERE Id = @AccountId
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
    WHERE Id = @AccountId
      AND IsDeleted = 0
      AND IsActive = 1;
END;
GO