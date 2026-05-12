CREATE TABLE CategoryAttributes (
    CategoryID INT NOT NULL,
    AttributeID INT NOT NULL,
    OrderPosition INT NOT NULL DEFAULT 0,
    FOREIGN KEY (CategoryID) REFERENCES Categories(ID),
    FOREIGN KEY (AttributeID) REFERENCES Attributes(ID),
    PRIMARY KEY(CategoryID, AttributeID)
);

