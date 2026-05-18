create procedure dbo.sp_GetAllEmployees
as 
begin 
    set nocount on;
    select * from Employees where IsDeleted = 0;
        
    return 0;
end