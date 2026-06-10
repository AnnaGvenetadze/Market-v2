create procedure dbo.sp_InsertSale
    @CreatedEmployeeId int,
    @Status tinyint = 0,                   
    @CreatedDate datetime = null,
    @CancelledByEmployeeId int = null,
    @CancelledDate datetime = null,
    @CancelReason nvarchar(200) = null,
    @Id int output
as
begin
    set nocount on;

    if not exists (select 1 from Employees where Id = @CreatedEmployeeId) throw 50101, 'Employee not found.', 1;
    insert into Sales (CreatedEmployeeId,Status,CreatedDate,CancelledByEmployeeId,CancelledDate,CancelReason)
    values (@CreatedEmployeeId,@Status,isnull(@CreatedDate, getdate()),@CancelledByEmployeeId,@CancelledDate,@CancelReason);

    set @Id = scope_identity();
    return 0;
end

