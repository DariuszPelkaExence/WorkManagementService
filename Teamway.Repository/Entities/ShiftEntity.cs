using System;
using Teamway.WorkManagementService.Repository;

namespace Teamway.WorkManagementService.Repository.Entities
{
    internal class ShiftEntity
    {
        public int Id { get; set; }

        public DateTime Day { get; set; }

        public ShiftType Type { get; set; }

        public int WorkerId { get; set; }
    }
}
