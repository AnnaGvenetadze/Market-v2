create or alter procedure dbo.sp_ActivateCategory
    @CategoryID int
as
begin
    set nocount on;

    if exists (
        select 1
        from dbo.Categories c
        inner join dbo.Categories p
            on c.ParentID = p.ID
        where c.ID = @CategoryID
          and p.IsActive = 0
    )
    begin
        raiserror('parent category is inactive.', 16, 1);
        return;
    end;

    update dbo.Categories
    set
        IsActive = 1,
        UpdatedDate = getdate()
    where ID = @CategoryID;

    if @@rowcount = 0
    begin
        raiserror('category was not found.', 16, 1);
        return;
    end;
end;
go