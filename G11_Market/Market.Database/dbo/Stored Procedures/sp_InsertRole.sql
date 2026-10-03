CREATE   PROCEDURE dbo.sp_InsertRole
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX) = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Roles (Name, Description, IsDeleted, CreateDate)
    VALUES (@Name, @Description, 0, GETUTCDATE());

    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;
