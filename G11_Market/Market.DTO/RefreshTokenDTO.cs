namespace Market.DTO;

public class RefreshTokenDTO
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiredAt { get; set; }
    public bool IsCancelled { get; set; }
}