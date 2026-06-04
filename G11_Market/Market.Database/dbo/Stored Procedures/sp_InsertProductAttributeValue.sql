create procedure sp_InsertProductAttributeValue
    @productId INT NOT NULL,
    @AttributeId INT NOT NULL,
    @TextValue NVARCHAR(500) = NULL,
    @NumberValue DECIMAL(18,2) = NULL,
    @DateValue DATETIME = NULL,
    @BooleanValue BIT = NULL
as
begin
    set nocount on;

    declare @AttributeType TinyInt;
    select @AttributeType = AttributeType
    from 
    Attributes Where AttributeId = Attributes.AttributeId;

    if @AttributeType is null
    begin
        raiserror('Invalid AttributeID', 16, 1);
        return -1;
    end;

        if @AttributeType = 1 and @TextValue is null
        raiserror('Text value required', 16, 2);

    if @AttributeType = 2 and @NumberValue is null
        raiserror('Number value required', 16, 3);

    if @AttributeType = 3 and @DateValue is null
        raiserror('Date value required', 16, 4);

    if @AttributeType = 4 and @BooleanValue is null
        raiserror('Boolean value required', 16, 5);

    if @AttributeType <> 1 and @TextValue is not null
        throw 50000, 'TextValue not allowed for this attribute type', 1;

    if @AttributeType <> 2 and @NumberValue is not null
        throw 50002, 'NumberValue not allowed for this attribute type', 1;

    if @AttributeType <> 3 and @DateValue is not null
        throw 50003, 'DateValue not allowed for this attribute type', 1;

    if @AttributeType <> 4 and @BooleanValue is not null
        throw 50004, 'BooleanValue not allowed for this attribute type', 1;

    insert into ProductAttributeValues (ProductId,AttributeId,TextValue,NumberValue,DateValue,BooleanValue)
    values (@productId,@AttributeId,@TextValue,@NumberValue,@DateValue,@BooleanValue);
    return 0;
end;
go
