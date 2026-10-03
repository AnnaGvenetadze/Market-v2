CREATE   PROCEDURE dbo.sp_UpdateEmployee
    @Id INT,
    @AccountId INT,
    @ManagerEmployeeId INT = NULL,
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @PhoneNumber NVARCHAR(20),
    @Email NVARCHAR(50),
    @EmployeeCode NVARCHAR(50),
    @HireDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    SET @FirstName = NULLIF(LTRIM(RTRIM(@FirstName)), '');
    SET @LastName = NULLIF(LTRIM(RTRIM(@LastName)), '');
    SET @PhoneNumber = NULLIF(LTRIM(RTRIM(@PhoneNumber)), '');
    SET @Email = NULLIF(LTRIM(RTRIM(@Email)), '');
    SET @EmployeeCode = NULLIF(LTRIM(RTRIM(@EmployeeCode)), '');

    IF @Email IS NULL THROW 50000, 'Email is required.', 1;
    IF @PhoneNumber IS NULL THROW 50000, 'PhoneNumber is required.', 1;
    IF @ManagerEmployeeId = @Id THROW 50000, 'Employee cannot manage themselves.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Employees WHERE AccountId = @AccountId AND Id <> @Id AND IsDeleted = 0) THROW 50000, 'Account is already assigned to an employee.', 1;
    IF @Id IS NULL THROW 50035, 'Employee ID is required for updates.', 1;
    IF @AccountId IS NULL THROW 50013, 'AccountId is required.', 1;
    IF @FirstName IS NULL THROW 50010, 'FirstName cannot be empty.', 1;
    IF @LastName IS NULL THROW 50011, 'LastName cannot be empty.', 1;
    IF @EmployeeCode IS NULL THROW 50023, 'EmployeeCode cannot be empty.', 1;
    IF @HireDate IS NULL THROW 50024, 'HireDate is required.', 1;
    IF @Email IS NOT NULL AND @Email NOT LIKE '%_@_%._%' THROW 50015, 'Invalid email format.', 1;
    IF @PhoneNumber IS NOT NULL AND @PhoneNumber LIKE '%[^0-9+ -]%' THROW 50020, 'PhoneNumber contains invalid characters.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Id = @Id AND IsDeleted = 0) THROW 50036, 'Employee not found or has been deleted.', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE Id = @AccountId AND IsDeleted = 0) THROW 50016, 'Invalid AccountId.', 1;
    IF @ManagerEmployeeId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Employees WHERE Id = @ManagerEmployeeId AND IsDeleted = 0) THROW 50025, 'Invalid ManagerEmployeeId.', 1;
    IF EXISTS (SELECT 1 FROM dbo.Employees WHERE EmployeeCode = @EmployeeCode AND Id <> @Id AND IsDeleted = 0) THROW 50026, 'EmployeeCode already exists.', 1;
    IF @Email IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Employees WHERE Email = @Email AND Id <> @Id AND IsDeleted = 0) THROW 50021, 'Email already exists.', 1;
    IF @PhoneNumber IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Employees WHERE PhoneNumber = @PhoneNumber AND Id <> @Id AND IsDeleted = 0) THROW 50022, 'Phone number already exists.', 1;

    UPDATE dbo.Employees
    SET AccountId = @AccountId,
        ManagerEmployeeId = @ManagerEmployeeId,
        FirstName = @FirstName,
        LastName = @LastName,
        PhoneNumber = @PhoneNumber,
        Email = @Email,
        EmployeeCode = @EmployeeCode,
        HireDate = @HireDate,
        UpdateDate = GETUTCDATE()
    WHERE Id = @Id AND IsDeleted = 0;
END;