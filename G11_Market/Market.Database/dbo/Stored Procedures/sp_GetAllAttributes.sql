CREATE   PROCEDURE dbo.sp_GetAllAttributes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, AttributeName, AttributeType, IsDeleted, CreatedDate, UpdatedDate
    FROM dbo.Attributes 
    WHERE IsDeleted = 0;
END;