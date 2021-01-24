using System;

namespace Teamway.WorkManagementService.Repository.Model
{
    public class AddShift
    {
        public DateTime Day { get; set; }

        public ShiftType Type { get; set; }

        public int WorkerId { get; set; }
    }
}
