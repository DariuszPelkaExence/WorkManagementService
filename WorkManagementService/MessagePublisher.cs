using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teamway.WorkManagementService.Repository.Model;

namespace Teamway.WorkManagementService.API
{
    public class MessagePublisher : IMessagePublisher
    {
        public void SendMessageShiftCreated(Shift shift)
        {
            Console.WriteLine("Shift created");
            // here we should send shift created message ...
        }

        public void SendMessageShiftRemoved(Shift shift)
        {
            Console.WriteLine("Shift removed");
            // here we should send shift removed message ...
        }
    }
}
