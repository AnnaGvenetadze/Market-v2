using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Market.DTO;
using Market.Services.Interfaces.Services;

namespace Market.Services;

public class ProductService : IProductService
{
    public ProductDTO CreateProduct(string sessionToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
        {
            throw new ArgumentException("Session token cannot be null or empty.", nameof(sessionToken));
        }

        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(sessionToken))
        {
            throw new ArgumentException("Invalid JWT token format.", nameof(sessionToken));
        }

        var jwtToken = handler.ReadJwtToken(sessionToken);

        bool hasAdminRole = jwtToken.Claims.Any(claim =>
            (claim.Type == ClaimTypes.Role || claim.Type == "role") &&
            claim.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase));

        if (!hasAdminRole)
        {
            throw new UnauthorizedAccessException("User does not have the required 'Admin' role to create a product.");
        }

        return new ProductDTO { Id = 1 };
    }
}