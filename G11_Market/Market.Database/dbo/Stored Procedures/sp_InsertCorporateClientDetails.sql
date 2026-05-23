create procedure dbo.sp_InsertCorporateClientDetails
    @CompanyName nvarchar(255),
    @TaxNumber nvarchar(100),
    @LegalAddress nvarchar(max),
    @ContactPersonName nvarchar(255),
    @Id int output
as
begin
    set nocount on;
    set @CompanyName = nullif(trim(@CompanyName), '');
    set @TaxNumber = nullif(trim(@TaxNumber), '');
    set @LegalAddress = nullif(trim(@LegalAddress), '');
    set @ContactPersonName = nullif(trim(@ContactPersonName), '');
    if @CompanyName is null throw 50004, 'CompanyName cannot be empty.', 1;
    if @TaxNumber is null throw 50005, 'TaxNumber cannot be empty.', 1;

    insert into CorporateClientDetails (CompanyName, TaxNumber, LegalAddress, ContactPersonName)
    values (@CompanyName, @TaxNumber, @LegalAddress, @ContactPersonName);

    set @Id = scope_identity();

    return 0;
end