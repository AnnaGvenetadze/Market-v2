CREATE PROCEDURE dbo.sp_InsertRole
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Roles
    (
        Name,
        Description
    )
    VALUES
    (
        @Name,
        @Description
    );

    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;
GO
