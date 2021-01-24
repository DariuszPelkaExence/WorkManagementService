using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Teamway.WorkManagementService.Repository.Model;


namespace Teamway.WorkManagementService.Repository
{
    public interface IRepository
    {
        Task<int> AddShift(AddShift shift);

        Task<IList<Shift>> GetShiftsPerWorker(int workerId);

        Task<Shift> GetShift(int shiftId);

        Task<Worker> GetWorker(int workerId);

        Task<RemoveShiftStatus> RemoveShift(int shiftId);

        Task<AddWorkerStatus> AddWorker(Worker worker);
    }
}
