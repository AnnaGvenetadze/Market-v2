CREATE PROCEDURE dbo.sp_GetAllClientTypes
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
    FROM dbo.ClientTypes
    WHERE IsDeleted = 0;
END;
GO
