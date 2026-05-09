create procedure dbo.sp_InsertAccount
@Username nvarchar(50),
@PasswordHash nvarchar(255),
@Email nvarchar(255),
@FirstName nvarchar(100),
@LastName nvarchar(100),
@AccountType tinyint, -- 1=Employee, 2=Individual Client, 3=Corporate Client
@PhoneNumber varchar(20) = null,
@ClientTypeId int = null,
@EmployeeCode nvarchar(50) = null,
@ManagerEmployeeId int = null,
@HireDate date = null,
@CompanyName nvarchar(255) = null,
@TaxNumber nvarchar(100) = null,
@LegalAddress nvarchar(max) = null,
@Id int output
as 
begin 
    set nocount on;
    begin try
        begin tran;
        if @AccountType not in (1, 2, 3) throw 50000, 'Invalid account type. Use 1 for Employee, 2 for Individual Client, 3 for Corporate Client', 1;
        insert into Accounts (Username, PasswordHash, Email, FirstName, LastName, AccountType, IsDeleted, CreateDate)
        values (@Username, @PasswordHash, @Email, @FirstName, @LastName, @AccountType, 0, getdate());
        set @Id = scope_identity();

        if @AccountType = 2 
        begin
            if @ClientTypeId is null throw 50001, 'ClientTypeId is required for Client accounts', 1;
            exec sp_InsertClient @AccountId = @Id, @ClientTypeId = @ClientTypeId, @FirstName = @FirstName, @LastName = @LastName, @PhoneNumber = @PhoneNumber;
        end
        
        else if @AccountType = 1
        begin
        if @EmployeeCode is null or @HireDate is null throw 50002, 'EmployeeCode and HireDate are required for Employee accounts', 1;
            exec sp_InsertEmployee @AccountId = @Id, @FirstName = @FirstName, @LastName = @LastName, @EmployeeCode = @EmployeeCode, @HireDate = @HireDate, @PhoneNumber = @PhoneNumber, @ContactEmail = @Email, @ManagerEmployeeId = @ManagerEmployeeId;
        end

        else if  @AccountType = 3
        begin 
            if @CompanyName is null or @TaxNumber is null throw 50003, 'CompanyName and TaxNumber are required for corporate accounts.', 1;
            exec sp_InsertCorporateClientDetails @AccountId = @Id, @CompanyName = @CompanyName, @TaxNumber = @TaxNumber, @LegalAddress = @LegalAddress, @ContactPersonName = @FirstName;
        end
        
        else throw 50004, 'Invalid AccountType provided.', 1;
        commit tran;
    end try
    begin catch
        if @@trancount > 0 rollback tran;
        throw;
    end catch
    return 0;
end
    