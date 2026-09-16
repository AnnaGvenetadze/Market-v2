create procedure sp_GetAllAttributes
as 
begin 
    set nocount on;
    select * from Attributes where IsDeleted = 0;
        
    return 0;
end
