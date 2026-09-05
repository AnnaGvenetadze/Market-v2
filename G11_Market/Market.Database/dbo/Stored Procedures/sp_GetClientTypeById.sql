CREATE PROCEDURE dbo.sp_GetClientTypeById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.ClientTypes
        WHERE Id = @Id
          AND IsDeleted = 0
    )
    BEGIN
        RAISERROR('Client type with Id %d was not found or has been deleted.', 16, 1, @Id);
        RETURN;
    END;

    SELECT
        Id,
        Name,
        Description,
        IsDeleted,
        CreateDate,
        UpdateDate
    FROM dbo.ClientTypes
    WHERE Id = @Id
      AND IsDeleted = 0;
END;
GO
