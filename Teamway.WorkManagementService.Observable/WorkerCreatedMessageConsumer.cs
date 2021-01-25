using System;
using System.Collections.Generic;
using System.Text;
using Teamway.WorkManagementService.Repository;
using Teamway.WorkManagementService.Repository.Model;

namespace Teamway.WorkManagementService.Observable
{
    public class WorkerCreatedMessageConsumer : IMessageConsumer
    {
        private readonly IRepository _repository;

        public WorkerCreatedMessageConsumer(IRepository repository)
        {
            _repository = repository;
        }

        public void ConsumeMessage(string message)
        {
            // Parse message to worker class
            var worker = new Worker();
            // Take message from message queue from microservice dealing with workers data management and parse worker
            _repository.AddWorker(worker);//Here code which adds new worker record to repository
        }
    }
}
