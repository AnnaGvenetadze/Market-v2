create or alter procedure dbo.sp_ValidateCategory
    @CategoryId int,
    @IsValid bit output,
    @Message nvarchar(500) output
as
begin
    set nocount on;

    set @IsValid = 0;
    set @Message = null;

    begin try
        if not exists
        (
            select 1
            from dbo.Categories
            where Id = @CategoryId
        )
        begin
            set @Message = N'Category was not found.';
            return;
        end;

        if not exists
        (
            select 1
            from dbo.Categories
            where Id = @CategoryId
              and IsActive = 1
        )
        begin
            set @Message = N'Category is not active.';
            return;
        end;

        set @IsValid = 1;
        set @Message = N'Category is valid.';
    end try
    begin catch
        set @IsValid = 0;
        set @Message = N'Category validation failed.';

        ;throw 50010, 'Category validation failed.', 1;
    end catch;
end;
go