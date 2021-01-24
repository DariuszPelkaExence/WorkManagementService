using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teamway.Repository.Model;

namespace Teamway.Repository
{
    public class Repository : IRepository
    {
        private static List<Shift> _shifts = new List<Shift>();
        private static List<Worker> _workers = new List<Worker>();

        public Shift GetShift(int shiftId)
        {
            return _shifts.FirstOrDefault(m => m.Id == shiftId);
        }

        public Worker GetWorker(int workerId)
        {
            return _workers.FirstOrDefault(m => m.Id == workerId);
        }

        public bool WorkerHasSameOrPreviousOrNextShift(int workerId, DateTime day, ShiftType type)
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

            return _shifts.Any(m => (m.WorkerId == workerId && m.Day == day && m.Type == type)
            || (m.WorkerId == workerId && m.Day == previousShiftDay && m.Type == previousShiftType)
            || (m.WorkerId == workerId && m.Day == nextShiftDay && m.Type == nextShiftType));
        }

        public IList<Shift> GetShiftsPerWorker(int workerId)
        {
            return _shifts.Where(m => m.Id == workerId).ToList();
        }

        public AddShiftStatus AddShift(Shift shift)
        {
            _shifts.Add(shift);

            return AddShiftStatus.Ok;
        }

        public RemoveShiftStatus RemoveShift(int shiftId)
        {
            var status = RemoveShiftStatus.Ok;

            var shift = _shifts.FirstOrDefault(m => m.Id == shiftId);

            if (shift != null)
            {
                _shifts.Remove(shift);
            }
            else
            {
                status = RemoveShiftStatus.RecordDoesNotExist;
            }

            return status;
        }

        public AddWorkerStatus AddWorker(Worker worker)
        {
            _workers.Add(worker);

            return AddWorkerStatus.Ok;
        }

        public RemoveWorkerStatus RemoveWorker(int workerId)
        {
            var status = RemoveWorkerStatus.Ok;

            var worker = _workers.FirstOrDefault(m => m.Id == workerId);

            if (worker != null)
            {
                _workers.Remove(worker);
            }
            else
            {
                status = RemoveWorkerStatus.WorkerDoesNotExist;
            }

            return status;
        }

        public AssignShiftToWorkerEnum AssignShiftToWorker(int shiftId, int workerId)
        {
            var status = AssignShiftToWorkerEnum.Ok;

            var shift = _shifts.FirstOrDefault(m => m.Id == shiftId);

            if (shift != null)
            {
                _shifts.Remove(shift);
            }
            else
            {
                status = AssignShiftToWorkerEnum.ShiftDoesNotExist;
            }

            var worker = _workers.FirstOrDefault(m => m.Id == workerId);

            if (shift != null)
            {
                _workers.Remove(worker);
            }
            else
            {
                status = AssignShiftToWorkerEnum.WorkerDoesNotExist;
            }

            return status;
        }

    }
}
