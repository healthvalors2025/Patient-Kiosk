using DoctorMobileApp.CommonClass;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientKiosk.Models;
using PatientKiosk.WebServices;

namespace PatientKiosk.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientSearchController : ControllerBase
    {
        //private readonly ILogger<PatientSearchController> _logger;
        private readonly PatientSearchService _patientSearchService;
        private readonly IDbConnectionFactory _db;
        private readonly IConfiguration _configuration;

        public PatientSearchController(PatientSearchService patientSearchService,IDbConnectionFactory db,IConfiguration configuration)
        {
            _patientSearchService = patientSearchService;
            _db = db;
            _configuration = configuration;
        }
        
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