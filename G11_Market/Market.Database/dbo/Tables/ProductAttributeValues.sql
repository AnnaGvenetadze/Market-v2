CREATE TABLE ProductAttributeValues (
    ProductId INT NOT NULL,
    AttributeId INT NOT NULL,
    TextValue NVARCHAR(500) NULL,
    NumberValue DECIMAL(18,2) NULL,
    DateValue DATETIME NULL,
    BooleanValue BIT NULL,
    FOREIGN KEY(AttributeId) REFERENCES Attributes(Id),
    FOREIGN KEY(ProductId) REFERENCES Products(Id),
    PRIMARY KEY(ProductId, AttributeId)
);

