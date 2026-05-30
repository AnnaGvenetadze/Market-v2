create procedure sp_GetProductAttributeValuesByProductId
	@ProductID int
as
begin
	set nocount on;

	if not exists (
		select 1
		from Products
		where ID = @ProductID
	)
	begin
		raiserror('Product not found.', 16,1 );
        return -1;
	end

    if not exists (
		select 1
		from ProductAttributeValues
		where ProductID = @ProductID
	)
	begin
		raiserror('Product values not found.', 16,1 );
        return -2;
	end

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
    from ProductAttributeValues as pav
    inner join Products as p
        on pav.ProductID = p.ID
    inner join Attributes as a
        on pav.AttributeID = a.ID
    where pav.ProductID = @ProductID
    order by a.AttributeName;

	return 0;
end