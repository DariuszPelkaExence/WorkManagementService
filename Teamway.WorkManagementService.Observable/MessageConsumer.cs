using System;
using System.Collections.Generic;
using System.Text;
using Teamway.WorkManagementService.Repository;
using Teamway.WorkManagementService.Repository.Model;

namespace Teamway.WorkManagementService.Observable
{
    public class MessageConsumer : IMessageConsumer
    {
        private readonly IRepository _repository;

        public MessageConsumer(IRepository repository)
        {
            _repository = repository;
        }

        public void ConsumeWorkerCreatedMessage()
        {
            var worker = new Worker();
            // Take message from message queue from microservice dealing with workers data management and parse worker
            _repository.AddWorker(worker);//Here code which adds new worker record to repository
        }
    }
}
