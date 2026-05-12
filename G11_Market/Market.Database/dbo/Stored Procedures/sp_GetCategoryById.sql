create or alter procedure dbo.sp_GetCategoryById
    @Id int
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
    where Id = @Id;
end;