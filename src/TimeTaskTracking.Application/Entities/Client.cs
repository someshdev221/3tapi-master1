namespace TimeTaskTracking.Models.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Skypeid { get; set; }
        public string? PhoneNumber { get; set; }
        public int DepartmentId { get; set; }
        public string? BillingAddress { get; set; }
        public string? Country { get; set; }
        public string? ClientCompanyName { get; set; }

    }
}
