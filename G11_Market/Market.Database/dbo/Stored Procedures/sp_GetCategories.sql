create or alter procedure dbo.sp_GetCategories
    @OnlyActive bit = 1
as
begin
    set nocount on;

    select
        Id,
        ParentId,
        ParentCategoryName,
        CategoryName,
        Description,
        IsActive,
        CreatedDate,
        UpdatedDate
    from dbo.vw_CategoriesWithParent
    where @OnlyActive = 0 or IsActive = 1
    order by ParentId, CategoryName;
end; 