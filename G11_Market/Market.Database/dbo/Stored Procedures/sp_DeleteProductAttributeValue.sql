--create procedure sp_DeleteProductAttributeValue
--	@ProductID int,
--	@AttributeID int
--as
--begin
--	set nocount on;

--	if not exists (
--		select 1
--		from Products
--		where ID = @ProductID
--	)
--	begin
--		raiserror('Product was not found.', 16, 1);
--		return -1;
--	end

--	if not exists (
--		select 1
--		from Attributes
--		where ID = @AttributeID
--	)
--	begin
--		raiserror('Attribute was not found.', 16, 1);
--		return -2;
--	end

--	if not exists (
--		select 1
--		from ProductAttributeValues
--		where AttributeID = @AttributeID
--		  AND ProductID = @ProductID
--	)
--	begin
--		raiserror('Attribute value was not found.', 16, 1);
--		return -3;
--	end

--	delete from ProductAttributeValues
--	where ProductID = @ProductID
--	  AND AttributeID = @AttributeID
		
--	return 0;
--end
