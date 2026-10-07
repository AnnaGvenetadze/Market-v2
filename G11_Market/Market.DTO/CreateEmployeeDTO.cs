namespace Market.DTO;

public class CreateEmployeeDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int? ManagerEmployeeId { get; set; }
}
