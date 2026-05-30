--create procedure sp_ActivateCategory
--    @CategoryID int
--as
--begin
--    set nocount on;

--    if not exists (
--        select 1
--        from Categories
--        where ID = @CategoryID
--    )
--    begin
--        raiserror('Category was not found.', 16, 1);
--        return -1;
--    end;

--    if exists (
--        select 1
--        from Categories
--        where ID = @CategoryID
--          and IsActive = 1
--    )
--    begin
--        raiserror('Category is already active.', 16, 1);
--        return -2;
--    end;

--    if exists (
--        select 1
--        from Categories c
--        inner join on Categories p
--            on c.ParentID = p.ID
--        where c.ID = @CategoryID
--          and p.IsActive = 0 
--    )
--    begin
--        raiserror('Parent category is inactive.', 16, 1);
--        return -3;
--    end;

--    update Categories
--    set
--        IsActive = 1,
--        UpdatedDate = GETDATE()
--    where ID = @CategoryID;

--    return 0;
--end