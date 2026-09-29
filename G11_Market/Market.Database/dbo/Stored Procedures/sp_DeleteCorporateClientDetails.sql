CREATE PROCEDURE sp_DeleteCorporateClientDetails
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id <= 0
        THROW 50000, 'Invalid CorporateClientDetails Id.', 1;

    UPDATE CorporateClientDetails
    SET IsDeleted = 1,
        UpdateDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;
END;