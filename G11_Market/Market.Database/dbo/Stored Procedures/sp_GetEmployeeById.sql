create procedure dbo.sp_GetEmployeeById
    @Id int
as 
begin 
    set nocount on;

    if @Id is null throw 50035, 'Employee ID is required.', 1;
    if not exists (select 1 from Employees where Id = @Id and IsDeleted = 0) throw 50036, 'Employee not found or has been deleted.', 1;

    select * from Employees where Id = @Id and IsDeleted = 0;
        
    return 0;
end