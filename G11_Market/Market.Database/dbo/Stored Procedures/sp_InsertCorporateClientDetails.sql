CREATE PROCEDURE sp_InsertCorporateClientDetails
    @Id INT OUTPUT,
    @CompanyName NVARCHAR(255),
    @TaxNumber NVARCHAR(100),
    @LegalAddress NVARCHAR(MAX) = NULL,
    @ContactPersonName NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id <= 0
        THROW 50000, 'Invalid Account Id.', 1;

    IF @CompanyName IS NULL OR TRIM(@CompanyName) = ''
        THROW 50000, 'Company Name cannot be empty.', 1;

    IF @TaxNumber IS NULL OR TRIM(@TaxNumber) = ''
        THROW 50000, 'Tax Number cannot be empty.', 1;

    IF NOT EXISTS (SELECT 1 FROM Accounts WHERE Id = @Id AND IsDeleted = 0)
        THROW 50000, 'Referenced Account does not exist or is deleted.', 1;

    IF EXISTS (SELECT 1 FROM CorporateClientDetails WHERE TaxNumber = TRIM(@TaxNumber) AND IsDeleted = 0)
        THROW 50000, 'A corporate client with this tax number already exists.', 1;

    INSERT INTO CorporateClientDetails (Id, CompanyName, TaxNumber, LegalAddress, ContactPersonName, IsDeleted, CreateDate)
    VALUES (@Id, TRIM(@CompanyName), TRIM(@TaxNumber), TRIM(@LegalAddress), TRIM(@ContactPersonName), 0, GETDATE());

    SELECT @Id AS Id;
END;