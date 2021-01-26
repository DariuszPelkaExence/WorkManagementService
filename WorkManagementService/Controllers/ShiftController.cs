using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Teamway.WorkManagementService.Repository;
using Teamway.WorkManagementService.Repository.Model;

namespace Teamway.WorkManagementService.API
{
  [System.Web.Http.Route("~/[controller]")]
    public class ShiftController : Controller
    {
        private readonly IRepository _repository;
        private readonly IMessagePublisher _messagePublisher;

        public ShiftController(IRepository repository, IMessagePublisher messagePublisher)
        {
            _repository = repository;
            _messagePublisher = messagePublisher;
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
            return shifts.Result.Any(m => (m.WorkerId == workerId && m.Day == day && m.Type == type)
                                    || (m.WorkerId == workerId && m.Day == previousShiftDay && m.Type == previousShiftType)
                                    || (m.WorkerId == workerId && m.Day == nextShiftDay && m.Type == nextShiftType));
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("Get", Name = "Get")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync(int shiftId)
        {
            var shift = await _repository.GetShift(shiftId);
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
        public async Task<IActionResult> GetShiftsPerWorkerAsync(int workerId)
        {
            var shifts = await _repository.GetShiftsPerWorker(workerId);
            return Ok(shifts);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("Add", Name = "Add")]
        public async Task<IActionResult> AddAsync(AddShift shift)
        {
            var worker = _repository.GetWorker(shift.WorkerId);

            if (worker.Result != null)
            {
                var shiftsExist = WorkerHasSameOrPreviousOrNextShift(shift.WorkerId, shift.Day, shift.Type);

                if (!shiftsExist)
                {
                    var id = _repository.AddShift(shift);

                    if (id.Result > 0)
                    {
                        var newShift = _repository.GetShift(id.Result);
                        await _messagePublisher.SendMessageShiftCreated(newShift.Result);
                        return Ok(id.Result);
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
        public async Task<IActionResult> RemoveAsync(int shiftId)
        {
            var removedShift = await _repository.GetShift(shiftId);
            await _messagePublisher.SendMessageShiftRemoved(removedShift);
            var operationStatus = await _repository.RemoveShift(shiftId);

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
