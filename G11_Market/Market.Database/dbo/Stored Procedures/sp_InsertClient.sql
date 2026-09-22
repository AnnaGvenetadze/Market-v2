CREATE PROCEDURE sp_InsertClient
    @AccountId INT,
    @ClientTypeId INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @PhoneNumber VARCHAR(20) = NULL,
    @ContactEmail NVARCHAR(255) = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @AccountId IS NULL OR @AccountId <= 0
        THROW 50000, 'Invalid Account Id.', 1;

    IF @ClientTypeId IS NULL OR @ClientTypeId <= 0
        THROW 50000, 'Invalid ClientType Id.', 1;

    IF @FirstName IS NULL OR TRIM(@FirstName) = ''
        THROW 50000, 'First Name cannot be empty.', 1;

    IF @LastName IS NULL OR TRIM(@LastName) = ''
        THROW 50000, 'Last Name cannot be empty.', 1;

    IF NOT EXISTS (SELECT 1 FROM Accounts WHERE Id = @AccountId AND IsDeleted = 0)
        THROW 50000, 'Referenced Account does not exist or is deleted.', 1;

    IF NOT EXISTS (SELECT 1 FROM ClientTypes WHERE Id = @ClientTypeId AND IsDeleted = 0)
        THROW 50000, 'Referenced ClientType does not exist or is deleted.', 1;

    INSERT INTO Clients (AccountId, ClientTypeId, FirstName, LastName, PhoneNumber, ContactEmail, IsDeleted, CreateDate)
    VALUES (@AccountId, @ClientTypeId, TRIM(@FirstName), TRIM(@LastName), TRIM(@PhoneNumber), TRIM(@ContactEmail), 0, GETDATE());

    SET @Id = SCOPE_IDENTITY();

    SELECT @Id AS Id;
END;