create procedure dbo.sp_UpdateCorporateClientDetails
    @AccountId int,
    @CompanyName nvarchar(255),
    @TaxNumber nvarchar(100),
    @LegalAddress nvarchar(max),
    @ContactPersonName nvarchar(255)
as 
begin 
    set nocount on;
    set @CompanyName = nullif(trim(@CompanyName), '');
    set @TaxNumber = nullif(trim(@TaxNumber), '');
    set @LegalAddress = nullif(trim(@LegalAddress), '');
    set @ContactPersonName = nullif(trim(@ContactPersonName), '');
    if @AccountId is null throw 50009, 'AccountId is required for updates.', 1;
    if @CompanyName is null throw 50004, 'CompanyName cannot be empty.', 1;
    if @TaxNumber is null throw 50005, 'TaxNumber cannot be empty.', 1;
    if not exists (select 1 from CorporateClientDetails where Id = @AccountId and IsDeleted = 0) throw 50032, 'Corporate client details not found or have been deleted.', 1;
    if exists (select 1 from CorporateClientDetails where TaxNumber = @TaxNumber and Id <> @AccountId and IsDeleted = 0) throw 50027, 'TaxNumber already exists.', 1;

    update CorporateClientDetails
    set CompanyName = @CompanyName,
        TaxNumber = @TaxNumber,
        LegalAddress = @LegalAddress,
        ContactPersonName = @ContactPersonName,
        UpdateDate = getdate()
    where Id = @AccountId;
      
    return 0;
end