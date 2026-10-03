CREATE   PROCEDURE dbo.sp_GetAllCategories
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ParentId, CategoryName, Description, IsDeleted, CreatedDate, UpdatedDate
    FROM dbo.Categories
    WHERE IsDeleted = 0;
END;