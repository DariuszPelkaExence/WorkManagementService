using System;
using System.Collections.Generic;
using Teamway.Repository.Model;


namespace Teamway.Repository
{
    public interface IRepository
    {
        int AddShift(AddShift shift);

        IList<Shift> GetShiftsPerWorker(int workerId);

        Shift GetShift(int shiftId);

        Worker GetWorker(int workerId);

        RemoveShiftStatus RemoveShift(int shiftId);

        AddWorkerStatus AddWorker(Worker worker);

        RemoveWorkerStatus RemoveWorker(int workerId);
    }
}
