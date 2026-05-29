using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientKiosk.Models;
using PatientKiosk.WebServices;

namespace PatientKiosk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientSearchController : ControllerBase
    {
        private readonly ILogger<PatientSearchController> _logger;
        private readonly PatientSearchService _patientSearchService;

        public PatientSearchController(PatientSearchService patientSearchService)
        {
            _patientSearchService = patientSearchService;
        }


       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("get-patient-detail")]
        public async Task<IActionResult> GetPatientSearchDeatil([FromBody] PatientSearchModel patientSearch)
        {
            var patientDetail = await _patientSearchService.GetPatientSearchListAsync(patientSearch);
            if (patientDetail == null || patientDetail.Count == 0)
            {
                return NotFound(new
                {
                    Status = false,
                    Message = "Patient Not Found"
                });
            }
            return Ok(new
            {
                Status = true,
                Message = "Success",
                Data = patientDetail
            });
        }
    }
}