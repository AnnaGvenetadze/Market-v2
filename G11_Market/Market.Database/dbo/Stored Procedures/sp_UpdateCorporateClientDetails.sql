CREATE PROCEDURE sp_UpdateCorporateClientDetails
    @Id INT,
    @CompanyName NVARCHAR(255),
    @TaxNumber NVARCHAR(100),
    @LegalAddress NVARCHAR(MAX) = NULL,
    @ContactPersonName NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id <= 0
        THROW 50000, 'Invalid CorporateClientDetails Id.', 1;

    IF @CompanyName IS NULL OR TRIM(@CompanyName) = ''
        THROW 50000, 'Company Name cannot be empty.', 1;

    IF @TaxNumber IS NULL OR TRIM(@TaxNumber) = ''
        THROW 50000, 'Tax Number cannot be empty.', 1;

    IF EXISTS (SELECT 1 FROM CorporateClientDetails WHERE TaxNumber = TRIM(@TaxNumber) AND Id <> @Id AND IsDeleted = 0)
        THROW 50000, 'A corporate client with this tax number already exists.', 1;

    UPDATE CorporateClientDetails
    SET CompanyName = TRIM(@CompanyName),
        TaxNumber = TRIM(@TaxNumber),
        LegalAddress = TRIM(@LegalAddress),
        ContactPersonName = TRIM(@ContactPersonName),
        UpdateDate = GETDATE()
    WHERE Id = @Id AND IsDeleted = 0;

    IF @@ROWCOUNT = 0
        THROW 50000, 'Corporate client details not found or already deleted.', 1;
END;