using System;
using System.Collections.Generic;
using System.Text;
using Teamway.Repository.Entities;
using Teamway.Repository.Model;
using AutoMapper;

namespace Teamway.WorkManagementService
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
