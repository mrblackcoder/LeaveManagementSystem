using LeaveManagementSystemNew.Data.Entities;

public class Account
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int EmployeeId { get; set; }
    public int Role { get; set; } 

    public Employee Employee { get; set; }
}
