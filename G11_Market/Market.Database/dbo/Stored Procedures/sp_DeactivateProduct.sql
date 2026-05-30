--create procedure sp_DeactivateProduct
--	@ProductID int
--as
--begin
--	set nocount on;

--	if not exists (
--		select 1
--		from Products
--		where ID = @ProductID
--	)
--	begin
--		raiserror('Product not found.', 16,1 );
--      return -1;
--	end

--	update Products
--	set
--		IsActive = 0,
--		UpdatedDate = GETDATE()
--	where ID = @ProductID

--	return 0;
--end