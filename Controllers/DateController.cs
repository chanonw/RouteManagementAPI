using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RouteAPI.Data;

namespace RouteAPI.Controllers
{
    [Route("api/[controller]")]
    public class DateController : Controller
    {
        private readonly IRouteRepository _repo;
        private readonly IConfiguration _config;
        public DateController(IRouteRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;

        }
        [HttpPost("checkUsedDate")]
        public async Task<IActionResult> checkedUsedDate([FromBody]Dto dto)
        {
            bool usedDate = await _repo.checkUsedDate(dto.transDate);
            if (usedDate == true)
            {
                return Ok(new {usedDate = true});
            }
            return Ok(new {usedDate = false});
        }
    }
}