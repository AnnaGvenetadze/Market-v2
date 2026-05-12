create or alter procedure dbo.sp_GetCategoryAttributes
    @CategoryId int
as
begin
    set nocount on;

    if not exists (
        select 1
        from dbo.Categories
        where Id = @CategoryId
    )
    begin
        ;throw 50041, 'Category was not found.', 1;
    end;

    if exists (
        select 1
        from dbo.Categories
        where Id = @CategoryId
          and IsActive = 0
    )
    begin
        ;throw 50042, 'Category is inactive.', 1;
    end;

    select
        ca.OrderPosition,
        pa.AttributeName
    from dbo.CategoryAttributes as ca
    inner join dbo.ProductAttributes as pa
        on ca.AttributeId = pa.Id
    where ca.CategoryId = @CategoryId
    order by ca.OrderPosition;
end;
go
