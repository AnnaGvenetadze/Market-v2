CREATE PROCEDURE sp_GetAllCities
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Cities
    WHERE IsDeleted = 0;
END