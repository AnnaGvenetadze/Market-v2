CREATE   PROCEDURE dbo.sp_UnassignCategoryAttribute
    @CategoryId INT,
    @AttributeId INT
AS
BEGIN
    SET NOCOUNT ON;
    EXEC dbo.sp_DeleteCategoryAttribute @CategoryId = @CategoryId, @AttributeId = @AttributeId;
END;