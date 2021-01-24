using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Web.Http;
using Teamway.Repository;
using Teamway.Repository.Model;

namespace Teamway.WorkManagementService.Controllers
{
  [System.Web.Http.Route("~/[controller]")]
    public class ShiftController : Controller
    {
        private readonly IRepository _repository;

        public ShiftController(IRepository repository)
        {
            _repository = repository;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("Get", Name = "Get")]
        [Produces("application/json")]
        public IActionResult Get(int shiftId)
        {
            var shift = _repository.GetShift(shiftId);
            if (shift != null)
            {
                return Ok(shift);
            }
            else
            {
                return NotFound();
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("GetWorkersShifts", Name = "GetWorkersShifts")]
        public IActionResult GetShiftsPerWorker(int workerId)
        {
            var shifts = _repository.GetShiftsPerWorker(workerId);
            return Ok(shifts);
        }
        [Microsoft.AspNetCore.Mvc.HttpPost("Add", Name = "Add")]
        public IActionResult Add(AddShift shift)
        {
            var worker = _repository.GetWorker(shift.WorkerId);

            if (worker != null)
            {
                var shiftsExist = _repository.WorkerHasSameOrPreviousOrNextShift(shift.WorkerId, shift.Day, shift.Type);

                if (!shiftsExist)
                {
                    var operationStatus = _repository.AddShift(shift);

                    if (operationStatus == AddShiftStatus.Ok)
                    {
                        return Ok();
                    }
                    else
                    {
                        var error = new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent("Overlapping shift exists", System.Text.Encoding.UTF8,
                                "text/plain"),
                            StatusCode = HttpStatusCode.NotFound
                        };
                        throw new HttpResponseException(error);
                    }
                }
                else
                {
                    var error = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Shift same, previous ot next exists", System.Text.Encoding.UTF8,
                            "text/plain"),
                        StatusCode = HttpStatusCode.NotFound
                    };
                    throw new HttpResponseException(error);
                }
            }
            else
            {

                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Worker doesn't exist", System.Text.Encoding.UTF8,
                        "text/plain"),
                    StatusCode = HttpStatusCode.NotFound
                };
                throw new HttpResponseException(response);
            }
        }


        [Microsoft.AspNetCore.Mvc.HttpDelete("Remove", Name = "Remove")]
        public IActionResult Remove(int workerId)
        {
            var operationStatus = _repository.RemoveWorker(workerId);

            if (operationStatus == RemoveWorkerStatus.Ok)
            {
                return Ok();
            }
            else
            {
                if (operationStatus == RemoveWorkerStatus.WorkerDoesNotExist)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("Worker doesn't exist", System.Text.Encoding.UTF8,
                            "text/plain"),
                        StatusCode = HttpStatusCode.NotFound
                    };
                    throw new HttpResponseException(response);
                }
                else
                {
                    var error = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("Worker record could not be removed", System.Text.Encoding.UTF8,
                            "text/plain"),
                        StatusCode = HttpStatusCode.NotFound
                    };
                    throw new HttpResponseException(error);
                }
            }
        }
    }
}
