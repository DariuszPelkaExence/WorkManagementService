using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teamway.Repository.Model
{
    public class Shift
    {
        public int Id { get; set; }

        public DateTime Day { get; set; }

        public ShiftType Type { get; set; }

        public int WorkerId { get; set; }

        public int ShiftId { get; set; }
    }
}
