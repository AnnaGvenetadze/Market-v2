create or alter procedure dbo.sp_InsertCategory
    @CategoryName nvarchar(100),
    @ParentId int = null,
    @Description nvarchar(1000) = null,
    @NewCategoryId int output
as
begin
    set nocount on;

    set @NewCategoryId = null;

    if @ParentId is not null
    begin
        declare @IsParentValid bit;
        declare @ParentValidationMessage nvarchar(300);

        exec dbo.sp_ValidateCategory
            @CategoryId = @ParentId,
            @IsValid = @IsParentValid output,
            @Message = @ParentValidationMessage output;

        if isnull(@IsParentValid, 0) = 0
        begin
            ;throw 50001, 'Parent category is invalid or does not exist.', 1;
        end;
    end;

    begin try
        insert into dbo.Categories
        (
            ParentID,
            CategoryName,
            Description
        )
        values
        (
            @ParentID,
            @CategoryName,
            @Description
        );

        set @NewCategoryID = convert(int, scope_identity());
    end try
    begin catch
        if error_number() in (2601, 2627)
        begin
            ;throw 50003, 'Category with the same name already exists.', 1;
        end;

        if error_number() = 547
        begin
            ;throw 50004, 'Category data violates table constraints.', 1;
        end;

        ;throw 50005, 'Category insert failed.', 1;
    end catch;
end;
go 
