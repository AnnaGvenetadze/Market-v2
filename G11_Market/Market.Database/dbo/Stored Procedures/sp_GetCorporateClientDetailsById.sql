create procedure dbo.sp_GetCorporateClientDetailsById
    @Id int
as 
begin 
    set nocount on;

    if @Id is null throw 50009, 'AccountId is required.', 1;
    if not exists (select 1 from CorporateClientDetails where Id = @Id and IsDeleted = 0) throw 50032, 'Corporate client details not found or have been deleted.', 1;

    select * from CorporateClientDetails where Id = @Id and IsDeleted = 0;
        
    return 0;
end