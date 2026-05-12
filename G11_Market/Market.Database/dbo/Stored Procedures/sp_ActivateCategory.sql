create or alter procedure dbo.sp_ActivateCategory
    @CategoryId int
as
begin
    set nocount on;

    if not exists (
        select 1
        from dbo.Categories
        where Id = @CategoryId
    )
    begin
        ;throw 50031, 'Category was not found.', 1;
    end;

    if exists (
        select 1
        from dbo.Categories
        where Id = @CategoryId
          and IsActive = 1
    )
    begin
        ;throw 50032, 'Category is already active.', 1;
    end;

    if exists (
        select 1
        from dbo.Categories c
        inner join dbo.Categories p
            on c.ParentId = p.Id
        where c.Id = @CategoryId
          and p.IsActive = 0 
    )
    begin
        ;throw 50033, 'Parent category is inactive.', 1;
    end;

    update dbo.Categories
    set
        IsActive = 1,
        UpdatedDate = getdate()
    where Id = @CategoryId;
end;
go

