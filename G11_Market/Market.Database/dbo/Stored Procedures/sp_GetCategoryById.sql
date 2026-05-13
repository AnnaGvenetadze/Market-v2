create or alter procedure dbo.sp_GetCategoryById
    @Id int
as
begin
    set nocount on;

    select *
    from Categories
    where Id = @Id and IsDeleted = 0;

    return 0;
end