CREATE PROCEDURE sp_InsertCity
    @Name NVARCHAR(100),
    @CountryId INT,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Name IS NULL OR TRIM(@Name) = ''
    BEGIN
        THROW 50000, 'City Name cannot be null, empty, or whitespace.', 1;
    END

    IF @CountryId IS NULL OR @CountryId <= 0
    BEGIN
        THROW 50000, 'Invalid Country Id.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM Countries WHERE Id = @CountryId AND IsDeleted = 0)
    BEGIN
        THROW 50000, 'Referenced Country does not exist or is deleted.', 1;
    END

    INSERT INTO Cities (Name, CountryId, IsDeleted, CreateDate)
    VALUES (TRIM(@Name), @CountryId, 0, GETDATE());

    SET @Id = SCOPE_IDENTITY();
END;