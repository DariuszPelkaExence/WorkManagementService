using System;
using System.Collections.Generic;
using Teamway.Repository.Model;


namespace Teamway.Repository
{
    public interface IRepository
    {
        bool WorkerHasSameOrPreviousOrNextShift(int workerId, DateTime day, ShiftType type);

        AddShiftStatus AddShift(Shift shift);

        IList<Shift> GetShiftsPerWorker(int workerId);

        Shift GetShift(int shiftId);

        Worker GetWorker(int workerId);

        RemoveShiftStatus RemoveShift(int shiftId);

        AddWorkerStatus AddWorker(Worker worker);

        RemoveWorkerStatus RemoveWorker(int workerId);

        AssignShiftToWorkerEnum AssignShiftToWorker(int shiftId, int workerId);
    }
}
