CREATE PROCEDURE sp_GetCityById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Cities
        WHERE Id = @Id
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('City with Id %d not found.', 16, 1, @Id);
        RETURN;
    END;

    SELECT *
    FROM Cities
    WHERE Id = @Id
      AND IsDeleted = 0;
END