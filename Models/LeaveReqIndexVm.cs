using LeaveManagementSystemNew.Data.Entities;

namespace LeaveManagementSystemNew.Models
{
    public class LeaveReqIndexVm
    {
        public int LeaveDuration { get; set; }

        public int Id { get; set; }

        public Employee Employee { get; set; }



    }
}
