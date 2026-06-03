create function dbo.fn_IsCircularCategory (
    @CategoryId int,
    @ParentId int
)
returns bit
as
begin
    if @CategoryId = @ParentId return 1;
    if @ParentId is null return 0;

    declare @SearchId int = @ParentId;
    while @SearchId is not null
    begin
        if @SearchId = @CategoryId return 1;
        select @SearchId = ParentId from Categories where Id = @SearchId;
    end

    return 0;
end;
