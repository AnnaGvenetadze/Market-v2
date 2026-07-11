CREATE PROCEDURE sp_UpdateCity
    @Id INT,
    @Name NVARCHAR(100),
    @CountryId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Cities
    SET
        Name = @Name,
        CountryId = @CountryId,
        UpdateDate = GETDATE()
    WHERE Id = @Id
      AND IsDeleted = 0;
END