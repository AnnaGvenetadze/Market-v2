namespace Market.DTO.DbEntities.InsertEntities
{
    public class AccountInsert
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte AccountType { get; set; } // 1 = Employee, 2 = Individual Client, 3 = Corporate Client
        public string? PhoneNumber { get; set; }
        public string? EmployeeCode { get; set; }
        public DateTime? HireDate { get; set; }
        public int? ManagerEmployeeId { get; set; }
        public int? ClientTypeId { get; set; }
        public string? CompanyName { get; set; }
        public string? TaxNumber { get; set; }
        public string? LegalAddress { get; set; }
        public string? ContactPersonName { get; set; }
    }
}
