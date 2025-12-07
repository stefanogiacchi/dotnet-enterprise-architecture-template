using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArcAI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {

        //TODO GET /health o /ping per liveness
    }
}
