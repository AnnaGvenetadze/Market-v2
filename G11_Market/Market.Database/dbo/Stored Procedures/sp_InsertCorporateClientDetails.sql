create procedure dbo.sp_InsertCorporateClientDetails
    @AccountId int,
    @CompanyName nvarchar(255),
    @TaxNumber nvarchar(100),
    @LegalAddress nvarchar(max) = null,
    @ContactPersonName nvarchar(255) = null
as
begin
    set nocount on;
    insert into CorporateClientDetails (Id, CompanyName, TaxNumber, LegalAddress, ContactPersonName)
    values (@AccountId, @CompanyName, @TaxNumber, @LegalAddress, @ContactPersonName);

    return 0;
end
