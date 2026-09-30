CREATE PROCEDURE dbo.sp_UpdateAccount
    @Id INT,
    @Username NVARCHAR(50) = NULL,
    @PasswordHash NVARCHAR(255) = NULL,
    @Email NVARCHAR(255) = NULL,
    @AccountType TINYINT = NULL,
    @FirstName NVARCHAR(50) = NULL,
    @LastName NVARCHAR(50) = NULL,
    @Hwid VARCHAR(64) = NULL,
    @FailedLoginAttempts INT = NULL,
    @LockoutTime DATETIME = NULL,
    @IsDeleted BIT = NULL,
    @CreateDate DATETIME = NULL,
    @UpdateDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Ensure the account exists and is active
    IF @Id IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE Id = @Id AND IsDeleted = 0)
    BEGIN
        RAISERROR('Account not found or has been deleted.', 16, 1);
        RETURN;
    END;

    -- 2. Validate empty strings only if explicitly passed
    IF @Username IS NOT NULL AND LTRIM(RTRIM(@Username)) = ''
    BEGIN
        RAISERROR('Username cannot be empty.', 16, 1);
        RETURN;
    END;

    IF @PasswordHash IS NOT NULL AND LTRIM(RTRIM(@PasswordHash)) = ''
    BEGIN
        RAISERROR('PasswordHash cannot be empty.', 16, 1);
        RETURN;
    END;

    IF @Email IS NOT NULL AND LTRIM(RTRIM(@Email)) = ''
    BEGIN
        RAISERROR('Email cannot be empty.', 16, 1);
        RETURN;
    END;

    IF @FirstName IS NOT NULL AND LTRIM(RTRIM(@FirstName)) = ''
    BEGIN
        RAISERROR('FirstName cannot be empty.', 16, 1);
        RETURN;
    END;

    IF @LastName IS NOT NULL AND LTRIM(RTRIM(@LastName)) = ''
    BEGIN
        RAISERROR('LastName cannot be empty.', 16, 1);
        RETURN;
    END;

    IF @AccountType IS NOT NULL AND @AccountType NOT IN (1, 2, 3)
    BEGIN
        RAISERROR('Invalid AccountType. Must be 1 (Employee), 2 (Individual Client), or 3 (Corporate Client).', 16, 1);
        RETURN;
    END;

    -- 3. Check duplicate constraints
    IF @Username IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Accounts WHERE Username = @Username AND Id <> @Id AND IsDeleted = 0)
    BEGIN
        RAISERROR('An active account with this username already exists.', 16, 1);
        RETURN;
    END;

    IF @Email IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Accounts WHERE Email = @Email AND Id <> @Id AND IsDeleted = 0)
    BEGIN
        RAISERROR('An active account with this email already exists.', 16, 1);
        RETURN;
    END;

    -- 4. Execute Update preserving existing table values when NULL is passed
    UPDATE dbo.Accounts
    SET 
        Username = ISNULL(@Username, Username),
        PasswordHash = ISNULL(@PasswordHash, PasswordHash),
        Email = ISNULL(@Email, Email),
        AccountType = ISNULL(@AccountType, AccountType),
        FirstName = ISNULL(@FirstName, FirstName),
        LastName = ISNULL(@LastName, LastName),
        Hwid = ISNULL(@Hwid, Hwid),
        FailedLoginAttempts = ISNULL(@FailedLoginAttempts, FailedLoginAttempts),
        LockoutTime = @LockoutTime,
        IsDeleted = ISNULL(@IsDeleted, IsDeleted),
        UpdateDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 0;
END;