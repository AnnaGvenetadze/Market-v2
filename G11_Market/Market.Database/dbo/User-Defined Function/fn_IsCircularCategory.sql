create function dbo.fn_IsCircularCategory (
    @CategoryID int,
    @ParentID int
)
returns bit
as
begin
    if @CategoryID = @ParentID return 1;
    if @ParentID is null return 0;

    declare @SearchID int = @ParentID;
    while @SearchID is not null
    begin
        if @SearchID = @CategoryID return 1;
        select @SearchID = ParentID from Categories where ID = @SearchID;
    end

    return 0;
end;
