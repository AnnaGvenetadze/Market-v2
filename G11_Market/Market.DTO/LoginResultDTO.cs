namespace Market.DTO;

public record LoginResultDTO
{
    public bool IsSuccess { get; init; }
    public int? UserId { get; init; }
    public string? ErrorMessage { get; init; }
}