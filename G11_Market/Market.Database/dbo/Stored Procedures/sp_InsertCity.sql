CREATE PROCEDURE sp_InsertCity
    @Name NVARCHAR(100),
    @CountryId INT,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Cities (Name, CountryId)
    VALUES (@Name, @CountryId);

    SET @Id = SCOPE_IDENTITY();
END