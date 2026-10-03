CREATE   PROCEDURE dbo.sp_GetCategoryById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ParentId, CategoryName, Description, IsDeleted, CreatedDate, UpdatedDate
    FROM dbo.Categories
    WHERE Id = @Id AND IsDeleted = 0;
END;