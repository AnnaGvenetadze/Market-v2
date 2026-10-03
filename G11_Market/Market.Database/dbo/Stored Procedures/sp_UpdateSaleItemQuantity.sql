CREATE   PROCEDURE dbo.sp_UpdateSaleItemQuantity
    @Id INT,
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;
    EXEC dbo.sp_UpdateSaleItem @Id = @Id, @Quantity = @Quantity;
END;
GO