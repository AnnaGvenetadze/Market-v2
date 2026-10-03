

CREATE   PROCEDURE dbo.sp_ActivateCategory
    @CategoryID INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.Categories c JOIN dbo.Categories p ON p.Id = c.ParentId
        WHERE c.Id = @CategoryID AND p.IsDeleted = 1) THROW 50000, 'Parent category is inactive.', 1;
    UPDATE dbo.Categories SET IsDeleted = 0, UpdatedDate = GETDATE() WHERE Id = @CategoryID;
    IF @@ROWCOUNT = 0 THROW 50000, 'Category not found.', 1;
END;