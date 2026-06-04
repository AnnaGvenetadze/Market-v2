create procedure sp_InsertClientType
	@Name nvarchar(100),
	@Description nvarchar(max),
	@Id int output
as
begin
	set nocount on;

	insert into ClientTypes(Name, Description)
	values (@Name, @Description);

	set @Id = scope_identity();

    return 0;
end