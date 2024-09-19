namespace LeaveManagementSystemNew.Models
{
    public class CreateEmployeeReqModel
    {
        public string Name { get; set; }

        public string? RegistrationNumber { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
