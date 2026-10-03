
CREATE   PROCEDURE dbo.sp_AssignCategoryAttribute
    @CategoryId INT,
    @AttributeId INT,
    @OrderPosition INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DummyId INT;
    EXEC dbo.sp_InsertCategoryAttribute @CategoryId = @CategoryId, @AttributeId = @AttributeId, @OrderPosition = @OrderPosition, @Id = @DummyId OUTPUT;
END;