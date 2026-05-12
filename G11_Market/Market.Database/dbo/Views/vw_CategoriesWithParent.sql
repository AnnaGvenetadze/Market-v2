create or alter view dbo.vw_CategoriesWithParent
as
select
    c.Id,
    c.ParentId,
    p.CategoryName as ParentCategoryName,
    c.CategoryName,
    c.Description,
    c.IsActive,
    c.CreatedDate,
    c.UpdatedDate
from dbo.Categories c
left join dbo.Categories p on c.ParentId = p.Id; 
