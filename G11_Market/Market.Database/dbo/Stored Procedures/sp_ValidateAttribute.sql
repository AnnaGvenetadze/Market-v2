create procedure sp_ValidateAttribute
    @AttributeID int
as
begin
    set nocount on;

    if not exists
    (
        select 1
        from Attributes
        where ID = @AttributeID
    )
    begin
        raiserror('Attribute was not found.', 16, 1);
        return -1;
    end;

    if exists
    (
        select 1
        from Attributes
        where ID = @AttributeID
          and IsActive = 0
    )
    begin
        raiserror('Attribute is inactive.', 16, 1);
        return -2;
    end;

    return 0;
end;