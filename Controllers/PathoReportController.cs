using Microsoft.AspNetCore.Mvc;
using PatientKiosk.Models;
using PatientKiosk.WebServices;

namespace PatientKiosk.Controllers
{
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
        [HttpPost]
        [Route("get-patho-report-list")]
        public async Task<IActionResult> GetPathoReportList([FromQuery] PathoReportRequestModel requestModel)
        {
            var pathoReportDetail = await _pathoReportService.GetPathoReportListAsync(requestModel);
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
