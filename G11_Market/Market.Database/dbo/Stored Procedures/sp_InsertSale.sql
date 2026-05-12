create procedure dbo.sp_InsertSale
    @CreatedEmployeeId int,
    @Id int output
as
begin
    set nocount on;

    if not exists (select 1 from Employees where Id = @CreatedEmployeeId) throw 50101, 'Employee not found.', 1;
    insert into Sales (CreatedEmployeeId,Status)
    values (@CreatedEmployeeId,0);

    set @Id = scope_identity();
    return 0;
end