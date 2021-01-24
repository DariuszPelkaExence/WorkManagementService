using System;
using System.Collections.Generic;
using System.Text;

namespace Teamway.Repository.Entities
{
    internal class ShiftEntity
    {
        public int Id { get; set; }

        public DateTime Day { get; set; }

        public ShiftType Type { get; set; }

        public int WorkerId { get; set; }
    }
}
