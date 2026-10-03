CREATE PROCEDURE dbo.sp_GetAllRoles
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Name,
        Description,
        IsDeleted,
        CreateDate,
        UpdateDate
    FROM dbo.Roles
    WHERE IsDeleted = 0;
END;
