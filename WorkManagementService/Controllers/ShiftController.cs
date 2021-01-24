using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teamway.Repository;
using Teamway.Repository.Model;

namespace Teamway.WorkManagementService.Controllers
{
  [Route("~/[controller]")]
    public class ShiftController : Controller
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IRepository _repository;

        public ShiftController(ILogger<WeatherForecastController> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet("Get", Name = "Get")]
        public Shift GetShift(int shiftId)
        {
            return _repository.GetShift(shiftId);
        }
    }
}
