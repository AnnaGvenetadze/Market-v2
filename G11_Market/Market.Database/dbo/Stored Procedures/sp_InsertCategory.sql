CREATE Procedure dbo.sp_InsertCategory
    @ParentId INT = NULL,
    @CategoryName NVARCHAR(100),
    @Description NVARCHAR(1000) = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @ParentId <= 0 SET @ParentId = NULL;

    IF @CategoryName IS NULL OR LTRIM(RTRIM(@CategoryName)) = ''
    BEGIN
        RAISERROR('Category name cannot be empty or whitespace.', 16, 1);
        RETURN;
    END

    IF @ParentId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Id = @ParentId AND IsDeleted = 0)
    BEGIN
        RAISERROR('Specified ParentId does not exist or is deleted.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryName = @CategoryName AND IsDeleted = 0)
    BEGIN
        RAISERROR('Category name already exists.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Categories (ParentId, CategoryName, Description)
    VALUES (@ParentId, @CategoryName, @Description);

    SET @Id = SCOPE_IDENTITY();

    SELECT @Id AS Id;
END;
