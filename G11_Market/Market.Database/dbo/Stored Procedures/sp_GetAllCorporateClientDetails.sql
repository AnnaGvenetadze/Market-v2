create procedure dbo.sp_GetAllCorporateClientDetails
as 
begin 
    set nocount on;
    select * from CorporateClientDetails where IsDeleted = 0;
        
    return 0;
end