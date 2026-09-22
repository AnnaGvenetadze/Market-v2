CREATE PROCEDURE sp_UpdateCity
    @Id INT,
    @Name NVARCHAR(100),
    @CountryId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id <= 0
    BEGIN
        THROW 50000, 'Invalid City Id.', 1;
    END

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

    UPDATE Cities
    SET Name = TRIM(@Name),
        CountryId = @CountryId,
        UpdateDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    -- 6. Verify row was updated
    IF @@ROWCOUNT = 0
    BEGIN
        THROW 50000, 'City not found or already deleted.', 1;
    END
END;