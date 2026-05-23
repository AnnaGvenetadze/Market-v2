--create procedure dbo.sp_CancelSale
--    @SaleId int,
--    @CancelledByEmployeeId int,
--    @CancelReason nvarchar(200)
--as
--begin
--    set nocount on;

--    set @CancelReason = trim(@CancelReason);
--    if @CancelReason is null or @CancelReason = '' throw 50122, 'CancelReason is required.', 1;

--    if not exists (select 1 from Sales where Id = @SaleId) throw 50123, 'Sale not found.', 1;
--    if not exists (select 1 from Employees where Id = @CancelledByEmployeeId) throw 50124, 'Cancelling employee not found.', 1;
--    if exists (select 1 from Sales where Id = @SaleId and Status = 1) throw 50125, 'Completed sale cannot be cancelled.', 1;
--    if exists (select 1 from Sales where Id = @SaleId and Status = 2) throw 50126, 'Sale is already cancelled.', 1;

--    update Sales
--    set
--        Status = 2,
--        CancelledByEmployeeId = @CancelledByEmployeeId,
--        CancelledAt = getdate(),
--        CancelReason = @CancelReason
--    where Id = @SaleId;

--    return 0;
--end