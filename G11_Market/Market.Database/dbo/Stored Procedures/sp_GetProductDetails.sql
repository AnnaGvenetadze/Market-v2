CREATE PROCEDURE dbo.sp_GetProductDetails
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Product
    SELECT
        p.Id,
        p.CategoryId,
        p.ProductName,
        p.Price,
        p.IsDeleted,
        p.CreatedDate,
        p.UpdatedDate
    FROM dbo.Products AS p
    WHERE p.Id = @ProductId;


    -- 2. Category
    SELECT
        c.Id,
        c.ParentId,
        c.CategoryName,
        c.Description,
        c.IsDeleted,
        c.CreatedDate,
        c.UpdatedDate
    FROM dbo.Products AS p
    INNER JOIN dbo.Categories AS c
        ON c.Id = p.CategoryId
    WHERE p.Id = @ProductId;


    -- 3. Category attributes + optional product values
    SELECT
        a.Id AS AttributeId,
        a.AttributeName,
        a.AttributeType,
        ca.OrderPosition,
        pav.TextValue,
        pav.NumberValue,
        pav.DateValue,
        pav.BooleanValue
    FROM dbo.Products AS p
    INNER JOIN dbo.CategoryAttributes AS ca
        ON ca.CategoryId = p.CategoryId
    INNER JOIN dbo.Attributes AS a
        ON a.Id = ca.AttributeId
       AND a.IsDeleted = 0
    LEFT JOIN dbo.ProductAttributeValues AS pav
        ON pav.ProductId = p.Id
       AND pav.AttributeId = a.Id
    WHERE p.Id = @ProductId
    ORDER BY
        ca.OrderPosition,
        a.Id;
END;