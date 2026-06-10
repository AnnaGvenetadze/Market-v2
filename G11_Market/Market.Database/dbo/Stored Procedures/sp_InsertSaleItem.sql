create procedure dbo.sp_InsertSaleItem
    @SaleId int,
    @ProductId int,
    @Quantity int,
    @UnitPrice money,
    @DiscountAmount money = 0,
    @Id int output
as
begin
    set nocount ON;

    insert into dbo.SaleItems
    (
        SaleId,
        ProductId,
        Quantity,
        UnitPrice,
        DiscountAmount
    )
    values
    (
        @SaleId,
        @ProductId,
        @Quantity,
        @UnitPrice,
        @DiscountAmount
    );

    set @Id = SCOPE_IDENTITY();

    return 0;
end;
go