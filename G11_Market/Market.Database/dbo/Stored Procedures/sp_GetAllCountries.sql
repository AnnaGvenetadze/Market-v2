CREATE PROCEDURE sp_GetAllCountries
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id, Name, CountryCode, CreateDate, UpdateDate
    FROM Countries
    WHERE IsDeleted = 0;
END;