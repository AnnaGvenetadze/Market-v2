CREATE   PROCEDURE dbo.sp_InsertEmployee
    @AccountId INT,
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @EmployeeCode NVARCHAR(50),
    @HireDate DATE,
    @PhoneNumber NVARCHAR(20), 
    @Email NVARCHAR(50),
    @ManagerEmployeeId INT = NULL,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @FirstName = NULLIF(LTRIM(RTRIM(@FirstName)), '');
    SET @LastName = NULLIF(LTRIM(RTRIM(@LastName)), '');
    SET @EmployeeCode = NULLIF(LTRIM(RTRIM(@EmployeeCode)), '');
    SET @Email = NULLIF(LTRIM(RTRIM(@Email)), '');
    SET @PhoneNumber = NULLIF(LTRIM(RTRIM(@PhoneNumber)), '');

    IF @Email IS NULL THROW 50000, 'Email is required.', 1;
    IF @PhoneNumber IS NULL THROW 50000, 'PhoneNumber is required.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Employees WHERE AccountId = @AccountId AND IsDeleted = 0) THROW 50000, 'Account is already assigned to an employee.', 1;
    IF @FirstName IS NULL THROW 50024, 'FirstName cannot be empty.', 1;
    IF @LastName IS NULL THROW 50025, 'LastName cannot be empty.', 1;
    IF @EmployeeCode IS NULL THROW 50026, 'EmployeeCode cannot be empty.', 1;
    IF @HireDate IS NULL THROW 50027, 'HireDate is required.', 1;
    IF @AccountId IS NULL THROW 50028, 'AccountId is required.', 1;
    IF @HireDate > GETUTCDATE() THROW 50019, 'HireDate cannot be in the future.', 1;
    IF @Email IS NOT NULL AND @Email NOT LIKE '%_@_%._%' THROW 50022, 'Invalid Email format.', 1;
    IF @ManagerEmployeeId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Id = @ManagerEmployeeId AND IsDeleted = 0) THROW 50018, 'Invalid ManagerEmployeeId.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Employees WHERE EmployeeCode = @EmployeeCode AND IsDeleted = 0) THROW 50020, 'EmployeeCode must be unique.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE Id = @AccountId AND IsDeleted = 0 AND IsActive = 1) THROW 50021, 'AccountId must reference a valid Employee account.', 1;

    INSERT INTO dbo.Employees (AccountId, ManagerEmployeeId, FirstName, LastName, PhoneNumber, Email, EmployeeCode, HireDate, IsDeleted, CreateDate)
    VALUES (@AccountId, @ManagerEmployeeId, @FirstName, @LastName, @PhoneNumber, @Email, @EmployeeCode, @HireDate, 0, GETUTCDATE());

    SET @Id = CONVERT(INT, SCOPE_IDENTITY());
END;