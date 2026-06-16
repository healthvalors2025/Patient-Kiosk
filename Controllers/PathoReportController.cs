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
    public class PathoReportController : ControllerBase
    {
        private readonly ILogger<PathoReportController> _logger;
        private readonly PathoReportService _pathoReportService;
        private readonly IDbConnectionFactory _db;
        private readonly IConfiguration _configuration;
        public PathoReportController(PathoReportService pathoReportService, IConfiguration configuration,IDbConnectionFactory db)
        {
            _pathoReportService = pathoReportService;
            _configuration = configuration;
            _db = db;
        }

        [HttpPost]
        [Route("get-patho-report-list-for-print")]
        public async Task<IActionResult> GetPathoReportListForPrint([FromBody] PathoReportRequestModel requestModel)
        {
            var pathoReportDetail = await _pathoReportService.GetPathoReportListForPrintAsync(requestModel);
            if (pathoReportDetail == null || pathoReportDetail.Count == 0)
            {
                return NotFound(new
                {
                    Status = false,
                    Message = "Pathology Report Not Found"
                });
            }
            return Ok(new
            {
                Status = true,
                Message = "Success",
                Data = pathoReportDetail
            });
        }
    }
}
