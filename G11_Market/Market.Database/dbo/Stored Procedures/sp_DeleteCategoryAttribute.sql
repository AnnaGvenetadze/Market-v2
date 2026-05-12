create or alter procedure dbo.sp_DeleteCategoryAttribute
    @CategoryId int,
    @AttributeId int
as
begin
    set nocount on;

    if not exists (
        select 1
        from dbo.Categories
        where Id = @CategoryId
    )
    begin
        ;throw 50051, 'Category was not found.', 1;
    end;

    if not exists (
        select 1
        from dbo.ProductAttributes
        where Id = @AttributeId
    )
    begin
        ;throw 50052, 'Attribute was not found.', 1;
    end;

    if not exists (
        select 1
        from dbo.CategoryAttributes
        where CategoryId = @CategoryId
          and AttributeId = @AttributeId
    )
    begin
        ;throw 50053, 'Category attribute relation was not found.', 1;
    end;

    if exists (
        select 1
        from dbo.Products p
        inner join dbo.ProductAttributeValues pav
            on pav.ProductId = p.Id
        where p.CategoryId = @CategoryId
          and pav.AttributeId = @AttributeId
    )
    begin
        ;throw 50054, 'Cannot remove attribute because products in this category already use it.', 1;
    end;

    delete from dbo.CategoryAttributes
    where CategoryId = @CategoryId
      and AttributeId = @AttributeId;
end;
go