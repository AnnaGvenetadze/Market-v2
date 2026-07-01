create procedure dbo.sp_GetCategories
as
begin
    set nocount on;

    select *
    from Categories
    where IsDeleted = 0;

    return 0;
end; 