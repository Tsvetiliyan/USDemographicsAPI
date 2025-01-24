using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using USDemographicsAPI.Core;
using USDemographicsAPI.Core.DomainModels;
using USDemographicsAPI.Core.Interfaces.IServices;

namespace USDemographicsAPI.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class USAPopulationController : Controller
    {
        //localhost:5001/api/v1/USAPopulation

        private readonly IStateService _stateService;

        public USAPopulationController(IStateService stateService)
        {
            _stateService = stateService;
        }
        [HttpGet("GetAllStatesDetails")]
        public async Task<IActionResult> GetAllStatesDetails()
        {
            Dictionary<State, int> statePop = await _stateService.GetAllStatesPop();
            var transformedStatePop = statePop
       .ToDictionary(kv => kv.Key.StateName, kv => kv.Value);
            return Json(transformedStatePop);
        }
        [HttpGet("GetSelectedStatesDetails")]
        public async Task<IActionResult> GetSelectedStatesDetails(string stateName)
        {
            Dictionary<State, int> statePop = await _stateService.GetSelectedStatePop(stateName);
            var transformedStatePop = statePop
               .ToDictionary(kv => kv.Key.StateName, kv => kv.Value);
            return Json(transformedStatePop);
        }
    }
}
