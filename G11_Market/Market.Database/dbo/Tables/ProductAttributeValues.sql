CREATE TABLE ProductAttributeValues (
    ProductID INT NOT NULL,
    AttributeID INT NOT NULL,
    TextValue NVARCHAR(500) NULL,
    NumberValue DECIMAL(18,2) NULL,
    DateValue DATETIME NULL,
    BooleanValue BIT NULL,
    FOREIGN KEY(AttributeID) REFERENCES Attributes(ID),
    FOREIGN KEY(ProductID) REFERENCES Products(ID),
    PRIMARY KEY(ProductID, AttributeID)
);

