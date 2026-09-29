CREATE PROCEDURE sp_GetCorporateClientDetailsById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM CorporateClientDetails
    WHERE Id = @Id AND IsDeleted = 0;
END;