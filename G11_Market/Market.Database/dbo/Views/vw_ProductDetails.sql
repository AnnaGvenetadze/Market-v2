CREATE VIEW dbo.vw_ProductDetails
AS
SELECT
    p.Id AS ProductId,
    p.ProductName,
    p.CategoryId,
    c.CategoryName,
    p.Price,
    p.IsDeleted,
    p.CreatedDate,
    p.UpdatedDate,

    a.Id AS AttributeId,
    a.AttributeName,
    a.AttributeType,

    ca.OrderPosition,

    pav.TextValue,
    pav.NumberValue,
    pav.DateValue,
    pav.BooleanValue,

    CASE a.AttributeType
        WHEN 1 THEN pav.TextValue
        WHEN 2 THEN CONVERT(NVARCHAR(100), pav.NumberValue)
        WHEN 3 THEN CONVERT(NVARCHAR(100), pav.DateValue, 23)
        WHEN 4 THEN
            CASE
                WHEN pav.BooleanValue = 1 THEN 'true'
                WHEN pav.BooleanValue = 0 THEN 'false'
                ELSE NULL
            END
    END AS AttributeValue
FROM Products p
INNER JOIN Categories c
    ON c.Id = p.CategoryId
LEFT JOIN CategoryAttributes ca
    ON ca.CategoryId = p.CategoryId
LEFT JOIN Attributes a
    ON a.Id = ca.AttributeId
LEFT JOIN ProductAttributeValues pav
    ON pav.ProductId = p.Id
   AND pav.AttributeId = a.Id
WHERE p.IsDeleted = 0
  AND c.IsDeleted = 0
  AND (a.Id IS NULL OR a.IsDeleted = 0);