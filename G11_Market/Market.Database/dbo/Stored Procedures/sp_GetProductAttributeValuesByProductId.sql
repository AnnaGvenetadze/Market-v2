create or alter procedure dbo.sp_GetProductAttributeValuesByProductId
    @ProductID int
as
begin
    set nocount on;

    if not exists (
        select 1
        from dbo.Products
        where ID = @ProductID
    )
    begin
        raiserror('product not found.', 16, 1);
        return;
    end;

    select
        pav.ProductID,
        p.ProductName,
        pav.AttributeID,
        a.AttributeName,
        a.AttributeType,
        pav.TextValue,
        pav.NumberValue,
        pav.DateValue,
        pav.BooleanValue
    from dbo.ProductAttributeValues as pav
    inner join dbo.Products as p
        on pav.ProductID = p.ID
    inner join dbo.Attributes as a
        on pav.AttributeID = a.ID
    where pav.ProductID = @ProductID
    order by a.AttributeName;
end;
go