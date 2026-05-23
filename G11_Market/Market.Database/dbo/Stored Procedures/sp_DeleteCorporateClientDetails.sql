create procedure dbo.sp_DeleteCorporateClientDetails
    @Id int
as 
begin 
    set nocount on;
    if @Id is null throw 50009, 'AccountId is required for deletion.', 1;
    if not exists (select 1 from CorporateClientDetails where Id = @Id and IsDeleted = 0) throw 50032, 'Corporate client details not found or have already been deleted.', 1;

    update CorporateClientDetails
    set IsDeleted = 1,
        UpdateDate = getdate()
    where Id = @Id;

    return 0;
end