create or alter procedure dbo.sp_GetCategories
as
begin
    set nocount on;

    select *
    from Categories
    where IsActive = 1;

    return 0;
end; 