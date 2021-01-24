using AutoMapper;
using Teamway.WorkManagementService.Repository.Entities;
using Teamway.WorkManagementService.Repository.Model;

namespace Teamway.WorkManagementService.Repository
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            base.CreateMap<AddShift, ShiftEntity>().ReverseMap();
            CreateMap<ShiftEntity, Shift>().ReverseMap();
            CreateMap<WorkerEntity, Worker>().ReverseMap();
        }

    }
}
