namespace LeaveManagementSystemNew.Data.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegistrationNumber { get; set; }

        public ICollection<Leave> Leaves { get; set; }
        public Account Account { get; set; }
    }
}
