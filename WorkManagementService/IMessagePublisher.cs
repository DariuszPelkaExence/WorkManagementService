using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teamway.WorkManagementService.Repository.Model;

namespace Teamway.WorkManagementService.API
{
    public interface IMessagePublisher
    {
        void SendMessageShiftCreated(Shift shift);

        void SendMessageShiftRemoved(Shift shift);
    }
}
