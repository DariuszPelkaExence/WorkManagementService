using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teamway.Repository.Model
{
    public class Shift
    {
        public int Id { get; set; }

        public DateTime Starts { get; set; }

        public DateTime Ends { get; set; }

        public int WorkerId { get; set; }

        public int ShiftId { get; set; }
    }
}
