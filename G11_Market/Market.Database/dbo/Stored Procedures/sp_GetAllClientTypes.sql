CREATE PROCEDURE dbo.sp_GetAllClientTypes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
    *
    FROM dbo.ClientTypes
    WHERE IsDeleted = 0;
END;
GO
