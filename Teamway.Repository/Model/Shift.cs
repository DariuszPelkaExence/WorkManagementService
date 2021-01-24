using System;

namespace Teamway.WorkManagementService.Repository.Model
{
    [Serializable]
    public class Shift
    {
        public int Id { get; set; }

        public DateTime Day { get; set; }

        public ShiftType Type { get; set; }

        public int WorkerId { get; set; }
    }
}
