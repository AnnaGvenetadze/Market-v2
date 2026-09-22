CREATE PROCEDURE sp_UpdateCountry
    @Id INT,
    @Name NVARCHAR(100),
    @CountryCode VARCHAR(3)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id <= 0
        THROW 50000, 'Invalid Country Id.', 1;

    IF @Name IS NULL OR TRIM(@Name) = ''
        THROW 50000, 'Country Name cannot be empty.', 1;

    IF @CountryCode IS NULL OR TRIM(@CountryCode) = '' OR LEN(TRIM(@CountryCode)) < 2
        THROW 50000, 'CountryCode must be 2 or 3 characters.', 1;

    IF EXISTS (SELECT 1 FROM Countries WHERE CountryCode = TRIM(@CountryCode) AND Id <> @Id AND IsDeleted = 0)
        THROW 50000, 'A country with this CountryCode already exists.', 1;

    UPDATE Countries
    SET Name = TRIM(@Name),
        CountryCode = TRIM(@CountryCode),
        UpdateDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
        THROW 50000, 'Country not found or already deleted.', 1;
END;