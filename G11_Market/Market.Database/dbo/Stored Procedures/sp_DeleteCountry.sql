CREATE PROCEDURE sp_DeleteCountry
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id <= 0
        THROW 50000, 'Invalid Country Id.', 1;

    UPDATE Countries
    SET IsDeleted = 1,
        UpdateDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
        THROW 50000, 'Country not found or already deleted.', 1;
END;