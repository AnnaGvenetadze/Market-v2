CREATE PROCEDURE dbo.sp_UpdateAccount
    @Id INT,
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(255),
    @Email NVARCHAR(255),
    @AccountType TINYINT,
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id <= 0
    BEGIN
        RAISERROR('Invalid Account Id.', 16, 1);
        RETURN;
    END;

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

    IF EXISTS (
        SELECT 1 
        FROM dbo.Accounts 
        WHERE Username = @Username 
          AND Id <> @Id 
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Another active account with this username already exists.', 16, 1);
        RETURN;
    END;

    IF EXISTS (
        SELECT 1 
        FROM dbo.Accounts 
        WHERE Email = @Email 
          AND Id <> @Id 
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Another active account with this email already exists.', 16, 1);
        RETURN;
    END;

    UPDATE dbo.Accounts
    SET Username = @Username,
        PasswordHash = @PasswordHash,
        Email = @Email,
        AccountType = @AccountType,
        FirstName = @FirstName,
        LastName = @LastName,
        UpdateDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Account with Id %d was not found or is inactive.', 16, 1, @Id);
        RETURN;
    END;
END;