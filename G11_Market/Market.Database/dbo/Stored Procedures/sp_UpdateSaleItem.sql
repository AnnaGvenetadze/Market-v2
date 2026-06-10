create procedure dbo.sp_UpdateSaleItem
	@Id int,
	@SaleId int,
	@ProductId int,
	@Quantity int,
	@UnitPrice money,
	@DiscountAmount money = 0

as
begin
	
	set nocount ON;

	update dbo.SaleItems
	set
		SaleId = @SaleId,
		ProductId = @ProductId,
		Quantity = @Quantity,
		UnitPrice = @UnitPrice,
		DiscountAmount = @DiscountAmount
	where Id = @Id;

	return 0;

end;
go