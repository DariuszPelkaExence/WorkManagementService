using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Teamway.Repository;
using Teamway.Repository.Model;

namespace Teamway.WorkPlanningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly ILogger<ShiftController> _logger;

        private readonly IRepository _repository;

        public ShiftController(ILogger<ShiftController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Shift Get(int shiftId)
        {
            var shift = _repository.GetShift(shiftId);
            return shift;
        }

        [HttpGet]
        public IList<Shift> GetShiftPerWorker(int workerId)
        {
            var shifst = _repository.GetShiftPerWorker(workerId);
            return shifst;
        }

        [HttpPost]
        public HttpStatusCode Add(Shift shift)
        { 
            var operationStatus = _repository.AddShift(shift);

            if (operationStatus == AddShiftStatus.Ok)
            {
                return HttpStatusCode.OK;
            }
            else
            {
                return HttpStatusCode.BadRequest;
            }
        }

        [HttpDelete]
        public System.Net.HttpStatusCode Remove(int workerId)
        {
            var operationStatus = _repository.RemoveWorker(workerId);

            if (operationStatus == RemoveWorkerStatus.Ok)
            {
                return HttpStatusCode.OK;
            }
            else
            {
                return HttpStatusCode.BadRequest;
            }
        }

    }
}
