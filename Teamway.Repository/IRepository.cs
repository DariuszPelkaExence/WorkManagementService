using System.Collections.Generic;
using Teamway.Repository.Model;


namespace Teamway.Repository
{
    public interface IRepository
    {
        AddShiftStatus AddShift(Shift shift);

        IList<Shift> GetShiftPerWorker(int workerId);

        Shift GetShift(int shiftId);

        RemoveShiftStatus RemoveShift(int shiftId);

        AddWorkerStatus AddWorker(Worker worker);

        RemoveWorkerStatus RemoveWorker(int workerId);

        AssignShiftToWorkerEnum AssignShiftToWorker(int shiftId, int workerId);
    }
}
