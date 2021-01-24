using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Teamway.WorkManagementService.Repository;
using Teamway.WorkManagementService.Repository.Model;

namespace Teamway.WorkManagementService.API
{
  [System.Web.Http.Route("~/[controller]")]
    public class ShiftController : Controller
    {
        private readonly IRepository _repository;

        public ShiftController(IRepository repository)
        {
            _repository = repository;
        }

        private bool WorkerHasSameOrPreviousOrNextShift(int workerId, DateTime day, ShiftType type)
        {
            DateTime previousShiftDay = day;
            ShiftType previousShiftType = type;
            DateTime nextShiftDay = day;
            ShiftType nextShiftType = type;

            switch (type)
            {
                case ShiftType.ShiftFrom0To8:
                    previousShiftDay = day.AddDays(-1);
                    previousShiftType = ShiftType.ShiftFrom16To24;
                    nextShiftDay = day;
                    nextShiftType = ShiftType.ShiftFrom8To16;
                    break;

                case ShiftType.ShiftFrom8To16:
                    previousShiftDay = day;
                    previousShiftType = ShiftType.ShiftFrom0To8;
                    nextShiftDay = day;
                    nextShiftType = ShiftType.ShiftFrom16To24;
                    break;

                case ShiftType.ShiftFrom16To24:
                    previousShiftDay = day;
                    previousShiftType = ShiftType.ShiftFrom8To16;
                    nextShiftDay = day.AddDays(1);
                    nextShiftType = ShiftType.ShiftFrom0To8;
                    break;
                default:
                    break;
            }

            var shifts = _repository.GetShiftsPerWorker(workerId);
            return shifts.Any(m => (m.WorkerId == workerId && m.Day == day && m.Type == type)
                                    || (m.WorkerId == workerId && m.Day == previousShiftDay && m.Type == previousShiftType)
                                    || (m.WorkerId == workerId && m.Day == nextShiftDay && m.Type == nextShiftType));
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
                var shiftsExist = WorkerHasSameOrPreviousOrNextShift(shift.WorkerId, shift.Day, shift.Type);

                if (!shiftsExist)
                {
                    var id = _repository.AddShift(shift);

                    if (id > 0)
                    {
                        return Ok(id);
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
                        StatusCode = HttpStatusCode.BadRequest
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
        public IActionResult Remove(int shiftId)
        {
            var operationStatus = _repository.RemoveShift(shiftId);

            if (operationStatus == RemoveShiftStatus.Ok)
            {
                return Ok();
            }
            else
            {
                var error = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Shift record could not be removed", System.Text.Encoding.UTF8,
                        "text/plain"),
                    StatusCode = HttpStatusCode.NotFound
                };
                throw new HttpResponseException(error);
            }
        }
    }
}
