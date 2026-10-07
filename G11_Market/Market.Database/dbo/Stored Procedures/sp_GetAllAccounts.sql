CREATE PROCEDURE dbo.sp_GetAllAccounts
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
    WHERE IsDeleted = 0;
END;
GO