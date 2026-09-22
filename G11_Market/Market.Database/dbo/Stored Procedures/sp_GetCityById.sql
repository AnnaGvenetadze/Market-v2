CREATE PROCEDURE sp_GetCityById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id, 
        Name, 
        CountryId, 
        IsDeleted, 
        CreateDate, 
        UpdateDate
    FROM Cities
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO