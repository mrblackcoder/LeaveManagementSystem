using LeaveManagementSystemNew.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystemNew.Models
{
    public class CreateLeaveReqModel
    {
        public CreateLeaveReqModel() { }

    
        public int EmployeeId { get; set; }
        public DateTime StartTime { get; set; }
        public int LeaveDuration { get; set; }

    }

}
