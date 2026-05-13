CREATE TABLE Attributes (
    ID INT PRIMARY KEY IDENTITY(1,1),
    AttributeName NVARCHAR(100) NOT NULL UNIQUE,
    AttributeType TINYINT NOT NULL CHECK(AttributeType in (1, 2, 3, 4)), -- 1: Text, 2: Number, 3: Date, 4: Boolean
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME NULL
);
