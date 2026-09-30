CREATE PROCEDURE dbo.sp_InsertAccount
    @Id INT = NULL OUTPUT,
    @Username NVARCHAR(50) = NULL,
    @PasswordHash NVARCHAR(255) = NULL,
    @Email NVARCHAR(255) = NULL,
    @AccountType TINYINT = NULL,
    @FirstName NVARCHAR(50) = NULL,
    @LastName NVARCHAR(50) = NULL,
    @Hwid VARCHAR(64) = NULL,
    @FailedLoginAttempts INT = 0,
    @LockoutTime DATETIME = NULL,
    @IsDeleted BIT = 0,
    @CreateDate DATETIME = NULL,
    @UpdateDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Username IS NULL OR LTRIM(RTRIM(@Username)) = ''
    BEGIN
        RAISERROR('Username cannot be empty.', 16, 1);
        RETURN;
    END;

    IF @PasswordHash IS NULL OR LTRIM(RTRIM(@PasswordHash)) = ''
    BEGIN
        RAISERROR('PasswordHash cannot be empty.', 16, 1);
        RETURN;
    END;

    IF @Email IS NULL OR LTRIM(RTRIM(@Email)) = ''
    BEGIN
        RAISERROR('Email cannot be empty.', 16, 1);
        RETURN;
    END;

    IF @FirstName IS NULL OR LTRIM(RTRIM(@FirstName)) = ''
    BEGIN
        RAISERROR('FirstName cannot be empty.', 16, 1);
        RETURN;
    END;

    IF @LastName IS NULL OR LTRIM(RTRIM(@LastName)) = ''
    BEGIN
        RAISERROR('LastName cannot be empty.', 16, 1);
        RETURN;
    END;

    IF @AccountType NOT IN (1, 2, 3)
    BEGIN
        RAISERROR('Invalid AccountType. Must be 1 (Employee), 2 (Individual Client), or 3 (Corporate Client).', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Accounts WHERE Username = @Username AND IsDeleted = 0)
    BEGIN
        RAISERROR('An active account with this username already exists.', 16, 1);
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Accounts WHERE Email = @Email AND IsDeleted = 0)
    BEGIN
        RAISERROR('An active account with this email already exists.', 16, 1);
        RETURN;
    END;

    INSERT INTO dbo.Accounts (
        Username, 
        PasswordHash, 
        Email, 
        AccountType, 
        FirstName, 
        LastName, 
        Hwid,
        FailedLoginAttempts,
        LockoutTime,
        IsDeleted,
        CreateDate,
        UpdateDate
    )
    VALUES (
        @Username, 
        @PasswordHash, 
        @Email, 
        @AccountType, 
        @FirstName, 
        @LastName, 
        ISNULL(@Hwid, ''),
        ISNULL(@FailedLoginAttempts, 0),
        @LockoutTime,
        ISNULL(@IsDeleted, 0),
        ISNULL(@CreateDate, GETUTCDATE()),
        @UpdateDate
    );

    SET @Id = SCOPE_IDENTITY();
END;