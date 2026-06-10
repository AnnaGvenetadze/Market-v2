create procedure dbo.sp_GetAllSales
as 
begin 
    set nocount on;
    select * from Sales;
        
    return 0;
end