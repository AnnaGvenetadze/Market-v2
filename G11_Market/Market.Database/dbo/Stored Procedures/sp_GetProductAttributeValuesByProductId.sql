create procedure dbo.sp_GetProductAttributeValuesByProductId
    @ProductId int
as
begin
    set nocount on;

    if not exists (
        select 1
        from dbo.Products
        where Id = @ProductId
    )
    begin
        raiserror('product not found.', 16, 1);
        return;
    end;

    select
        pav.ProductId,
        p.ProductName,
        pav.AttributeId,
        a.AttributeName,
        a.AttributeType,
        pav.TextValue,
        pav.NumberValue,
        pav.DateValue,
        pav.BooleanValue
    from dbo.ProductAttributeValues as pav
    inner join dbo.Products as p
        on pav.ProductId = p.Id
    inner join dbo.Attributes as a
        on pav.AttributeId = a.Id
    where pav.ProductId = @ProductId
    order by a.AttributeName;
end;
go