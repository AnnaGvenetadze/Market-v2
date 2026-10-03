CREATE   PROCEDURE dbo.sp_GetAttributeById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Attributes
    WHERE Id = @Id
      AND IsDeleted = 0;
END;