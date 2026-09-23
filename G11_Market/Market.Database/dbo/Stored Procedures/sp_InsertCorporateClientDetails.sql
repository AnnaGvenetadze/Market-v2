CREATE PROCEDURE dbo.sp_InsertCorporateClientDetails
    @Id INT OUTPUT,                       -- Accounts.Id შემოდის აქ
    @CompanyName NVARCHAR(255),
    @TaxNumber NVARCHAR(100),
    @LegalAddress NVARCHAR(MAX) = NULL,
    @ContactPersonName NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Id აუცილებელია, რადგან CorporateClientDetails.Id
    -- უნდა იყოს უკვე არსებული Account-ის Id.
    IF @Id IS NULL OR @Id <= 0
        THROW 50000, 'Invalid Account Id.', 1;


    IF @CompanyName IS NULL OR TRIM(@CompanyName) = ''
        THROW 50000, 'Company Name cannot be empty.', 1;


    IF @TaxNumber IS NULL OR TRIM(@TaxNumber) = ''
        THROW 50000, 'Tax Number cannot be empty.', 1;


    -- ასეთი Account რეალურად უნდა არსებობდეს
    -- და soft deleted არ უნდა იყოს.
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Accounts
        WHERE Id = @Id
          AND IsDeleted = 0
    )
        THROW 50000, 'Referenced Account does not exist or is deleted.', 1;


    -- რადგან CorporateClientDetails.Id არის PK,
    -- ერთ Account-ზე მეორე CorporateClientDetails ვერ შეიქმნება.
    IF EXISTS
    (
        SELECT 1
        FROM dbo.CorporateClientDetails
        WHERE Id = @Id
    )
        THROW 50000, 'Corporate client details already exist for this account.', 1;


    IF EXISTS
    (
        SELECT 1
        FROM dbo.CorporateClientDetails
        WHERE TaxNumber = TRIM(@TaxNumber)
          AND IsDeleted = 0
    )
        THROW 50000, 'A corporate client with this tax number already exists.', 1;


    INSERT INTO dbo.CorporateClientDetails
    (
        Id,
        CompanyName,
        TaxNumber,
        LegalAddress,
        ContactPersonName
    )
    VALUES
    (
        @Id,                              -- ახალი Id არ გენერირდება;
                                          -- გამოიყენება Accounts.Id
        TRIM(@CompanyName),
        TRIM(@TaxNumber),
        TRIM(@LegalAddress),
        TRIM(@ContactPersonName)
    );

    -- @Id OUTPUT-ია, ამიტომ C#-ში ისევ ამ Id-ს დავიბრუნებთ.
END;