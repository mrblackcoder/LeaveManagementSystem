using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystemNew.Data.Entities
{
    public class Leave
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public required DateTime StartTime { get;  set; } = DateTime.Now;

        [Required]
        public int LeaveDuration { get; set; }  




        public Employee Employee { get; set; }
    }
}
 