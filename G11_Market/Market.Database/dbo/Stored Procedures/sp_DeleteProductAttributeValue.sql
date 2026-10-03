
CREATE   PROCEDURE dbo.sp_DeleteProductAttributeValue
	@ProductId int,
	@AttributeId int
as
begin
	set nocount on;

	if not exists (
		select 1
		from Products
		where Id = @ProductId
	)
	begin
		raiserror('Product was not found.', 16, 1);
		return -1;
	end

	if not exists (
		select 1
		from Attributes
		where Id = @AttributeId
	)
	begin
		raiserror('Attribute was not found.', 16, 1);
		return -2;
	end

	if not exists (
		select 1
		from ProductAttributeValues
		where AttributeId = @AttributeId
		  AND ProductId = @ProductId
	)
	begin
		raiserror('Attribute value was not found.', 16, 1);
		return -3;
	end

	delete from ProductAttributeValues
	where ProductId = @ProductId
	  AND AttributeId = @AttributeId
		
	return 0;
end