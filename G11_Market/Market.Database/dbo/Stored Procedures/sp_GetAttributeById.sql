CREATE PROCEDURE sp_GetAttributeById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        AttributeName,
        AttributeType,
        CreatedDate,
        UpdatedDate
    FROM Attributes
    WHERE Id = @Id
      AND IsDeleted = 0;
END;