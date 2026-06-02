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

        public PathoReportController(PathoReportService pathoReportService)
        {
            _pathoReportService = pathoReportService;
        }

        [Authorize]
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
