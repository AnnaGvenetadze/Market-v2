namespace Market.DTO;

public sealed class LoginResultDTO
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public int? AccountId { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? RefreshTokenExpiredAt { get; init; }
}