namespace Market.DTO;

public record AuthResultDTO
{
    public bool IsSuccess { get; init; }
    public int? UserId { get; init; }
    public string? ErrorMessage { get; init; }
}