create procedure dbo.sp_DeleteEmployee
    @Id int
as 
begin 
    set nocount on;
    if @Id is null throw 50035, 'Employee ID is required for deletion.', 1;
    if not exists (select 1 from Employees where Id = @Id and IsDeleted = 0) throw 50036, 'Employee not found or has already been deleted.', 1;

    update Employees
    set IsDeleted = 1,
        UpdateDate = getdate()
    where Id = @Id;
        
    return 0;
end