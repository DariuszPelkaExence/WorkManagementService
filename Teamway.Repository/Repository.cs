using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Teamway.WorkManagementService.Repository.Entities;
using Teamway.WorkManagementService.Repository.Model;

namespace Teamway.WorkManagementService.Repository
{
    public class Repository : IRepository
    {
        private readonly IMapper _mapper;
        private static IList<ShiftEntity> _shifts = new List<ShiftEntity>();
        private static IList<WorkerEntity> _workers = new List<WorkerEntity>();

        public Repository(IMapper mapper)
        {
            _mapper = mapper;
            _workers.Add(new WorkerEntity(){Id = 1, FirstName = "John", LastName = "Smith"});
            _workers.Add(new WorkerEntity() { Id = 2, FirstName = "Sam", LastName = "Jackson" });
            _workers.Add(new WorkerEntity() { Id = 3, FirstName = "Terry", LastName = "Grant" });
        }

        public Shift GetShift(int shiftId)
        {
            var shift = _shifts.FirstOrDefault(m => m.Id == shiftId);
            return _mapper.Map<Shift>(shift);
        }

        public Worker GetWorker(int workerId)
        {
            var worker = _workers.FirstOrDefault(m => m.Id == workerId);
            return _mapper.Map<Worker>(worker);
        }

        public IList<Shift> GetShiftsPerWorker(int workerId)
        {
            var shifts = _shifts.Where(m => m.Id == workerId).ToList();
            return _mapper.Map<IList<Shift>>(shifts);
        }

        public int AddShift(AddShift shift)
        {
            var highestId = _shifts.Count == 0 ? 0 :_shifts.Max(m => m.Id);
            var entity = _mapper.Map<ShiftEntity>(shift);
            entity.Id = highestId + 1;
            _shifts.Add(entity);

            return entity.Id;
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
            _workers.Add(_mapper.Map<WorkerEntity>(worker));

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
    }
}
