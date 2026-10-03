CREATE   PROCEDURE dbo.sp_GetAttributeById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, AttributeName, AttributeType, IsDeleted, CreatedDate, UpdatedDate
    FROM dbo.Attributes
    WHERE Id = @Id AND IsDeleted = 0;
END;