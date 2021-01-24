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

        public IList<Shift> GetShiftPerWorker(int workerId)
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

            if( shift != null)
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
