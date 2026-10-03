
CREATE   PROCEDURE dbo.sp_UpdateCategory
    @Id INT,
    @ParentId INT = NULL,
    @CategoryName NVARCHAR(100),
    @Description NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ParentId = @Id
    BEGIN
        RAISERROR('A category cannot be its own parent.', 16, 1);
        RETURN;
    END

    IF @ParentId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Categories WHERE Id = @ParentId AND IsDeleted = 0)
    BEGIN
        RAISERROR('Specified ParentId does not exist or is deleted.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Categories WHERE CategoryName = @CategoryName AND Id <> @Id)
    BEGIN
        RAISERROR('Category name already exists.', 16, 1);
        RETURN;
    END

    UPDATE Categories
    SET ParentId = @ParentId,
        CategoryName = @CategoryName,
        Description = @Description,
        UpdatedDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Category with Id %d was not found or is deleted.', 16, 1, @Id);
    END;
END;