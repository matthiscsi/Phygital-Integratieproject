using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UI.MVC.Hub;

namespace UI.MVC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly IHubContext<FlowHub> _flowHubContext;

        public SensorController(IHubContext<FlowHub> flowHubContext)
        {
            _flowHubContext = flowHubContext;
        }

        [HttpPost("movement-detected")]
        public IActionResult MovementDetected()
        {
            Console.WriteLine("Movement Detected!");
            _flowHubContext.Clients.All.SendAsync("ResetTimer");


            return Ok();
        }
    }
}