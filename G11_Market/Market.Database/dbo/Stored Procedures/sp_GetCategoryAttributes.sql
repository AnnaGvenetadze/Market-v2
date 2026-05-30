create procedure sp_GetCategoryAttributes
    @categoryid int
as
begin
    set nocount on;

    if not exists (
        select 1
        from categories
        where id = @categoryid
    )
    begin
        raiserror('category was not found.', 16, 1);
        return -1;
    end;

    if exists (
        select 1
        from categories
        where id = @categoryid
          and isactive = 0
    )
    begin
        raiserror('category is inactive.', 16, 1);
        return -2;
    end;

    select
        ca.orderposition,
        pa.attributename
    from categoryattributes as ca
    inner join productattributes as pa
        on ca.attributeid = pa.id
    where ca.categoryid = @categoryid
    order by ca.orderposition;

    return 0;
end;