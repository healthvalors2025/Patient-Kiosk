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
    public class AdvanceDepositController : ControllerBase
    {
        //private readonly ILogger<AdvanceDepositController> _logger;
        private readonly AdvanceDepositService _advanceDepositService;
        private readonly IDbConnectionFactory _db;
        private readonly IConfiguration _configuration;
        public AdvanceDepositController(AdvanceDepositService advanceDepositService, IDbConnectionFactory db,IConfiguration configuration)
        {
            _db = db;   
            _configuration = configuration;
            _advanceDepositService = advanceDepositService;
        }

        [HttpPost]
        [Route("save-advance-deposit")]
        public async Task<IActionResult> SaveAdvanceDeposit([FromBody] AdvanceDepositModel depositmodel)
        {
            if (depositmodel == null) 
            { 
                return BadRequest(new 
                {
                    Status = false, 
                    Message = "Invalid Request" 
                }); 
            }
            var voucherId = await _advanceDepositService.SaveAdvanceDepositAsync(depositmodel);
            if (voucherId <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Failed"
                });
            }
            return Ok(new
            {
                Status = true,
                Message = "Advance Deposit Saved Successfully",
                VoucherID = voucherId
            });
        }
    }
}
