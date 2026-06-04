CREATE TABLE CategoryAttributes (
    CategoryId INT NOT NULL,
    AttributeId INT NOT NULL,
    OrderPosition INT NOT NULL DEFAULT 0,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    FOREIGN KEY (AttributeId) REFERENCES Attributes(Id),
    PRIMARY KEY(CategoryId, AttributeId)
);

