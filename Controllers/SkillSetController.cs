using DoctorMobileApp.CommonClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientKiosk.Models;
using PatientKiosk.WebServices;

namespace PatientKiosk.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SkillSetController : ControllerBase
    {
        private readonly SkillSetService _skillSetService;
        private readonly IDbConnectionFactory _db;
        private readonly IConfiguration _configuration;
        public SkillSetController(SkillSetService skillSetService, IConfiguration configuration,IDbConnectionFactory db)
        {
            _skillSetService = skillSetService;
            _configuration = configuration;
            _db = db;
        }

        [HttpPost]
        [Route("get-skill-set")]
        public async Task<IActionResult> GetSkillSet([FromBody] SkillSetRequestModel requestModel)
        {
            var skillSetList = await _skillSetService.GetSkillSetListAsync(requestModel);
            if (skillSetList == null || skillSetList.Count == 0)
            {
                return NotFound(new
                {
                    Status = false,
                    Message = "Record Not Found"
                });
            }
            return Ok(new
            {
                Status = true,
                Message = "Success",
                Data = skillSetList
            });
        }
    }
}
