create procedure dbo.sp_DeleteCategory
    @CategoryId int
as
begin
    set nocount on;

    declare @IsValid bit = 0;
    declare @ValidationMessage nvarchar(500) = null;

    exec dbo.sp_ValidateCategory
        @CategoryId = @CategoryId,
        @IsValid = @IsValid output,
        @Message = @ValidationMessage output;

    if @IsValid = 0
    begin
        if @ValidationMessage = N'Category is not active.'
        begin
            ;throw 50020, 'Category is already deactivated.', 1;
        end;

        ;throw 50021, 'Category is invalid or was not found.', 1;
    end;

    if exists
    (
        select 1
        from dbo.Categories
        where ParentId = @CategoryId
          and IsDeleted = 0
    )
    begin
        ;throw 50022, 'Category has active subcategories. Deactivate or remove subcategories first.', 1;
    end;

    if exists
    (
        select 1
        from dbo.Products
        where CategoryId = @CategoryId
          and IsDeleted = 0
    )
    begin
        ;throw 50023, 'Category has active products. Deactivate, delete, or move products to another category first.', 1;
    end;

    begin try
        update dbo.Categories
        set
            IsDeleted = 1,
            UpdatedDate = getdate()
        where Id = @CategoryId;

        if @@rowcount = 0
        begin
            ;throw 50024, 'Category deactivation failed because no category was updated.', 1;
        end;
    end try
    begin catch
        if error_number() between 50000 and 59999
        begin
            ;throw;
        end;

        ;throw 50025, 'Category deactivation failed.', 1;
    end catch;
end;
go