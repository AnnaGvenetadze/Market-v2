CREATE PROCEDURE dbo.sp_GetAccountById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Username,
        PasswordHash,
        Token,
        TokenExpiration,
        IsDeleted,
        CreateDate,
        UpdateDate,
        LastLoginAtUtc,
        FailedLoginAttempts,
        LockoutEndUtc,
        IsActive
    FROM dbo.Accounts
    WHERE Id = @Id
      AND IsDeleted = 0;
END;
GO