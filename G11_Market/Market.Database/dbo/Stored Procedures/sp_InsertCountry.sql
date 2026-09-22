CREATE PROCEDURE sp_InsertCountry
    @Name NVARCHAR(100),
    @CountryCode VARCHAR(3),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Name IS NULL OR TRIM(@Name) = ''
        THROW 50000, 'Country Name cannot be empty.', 1;

    IF @CountryCode IS NULL OR TRIM(@CountryCode) = '' OR LEN(TRIM(@CountryCode)) < 2
        THROW 50000, 'CountryCode must be 2 or 3 characters.', 1;

    IF EXISTS (SELECT 1 FROM Countries WHERE CountryCode = TRIM(@CountryCode) AND IsDeleted = 0)
        THROW 50000, 'A country with this CountryCode already exists.', 1;

    INSERT INTO Countries (Name, CountryCode, IsDeleted, CreateDate)
    VALUES (TRIM(@Name), TRIM(@CountryCode), 0, GETDATE());

    SET @Id = SCOPE_IDENTITY();

    SELECT @Id AS Id;
END;