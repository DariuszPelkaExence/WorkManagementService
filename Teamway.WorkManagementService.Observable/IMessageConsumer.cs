using System;
using System.Collections.Generic;
using System.Text;

namespace Teamway.WorkManagementService.Observable
{
    public interface IMessageConsumer
    {
        void ConsumeMessage(string message);
    }
}
