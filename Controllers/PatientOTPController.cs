using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PatientKiosk.Models;
using PatientKiosk.WebServices;

namespace PatientKiosk.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientOTPController : Controller
    {
        private readonly PatientOTPService patientOTPService;

        public PatientOTPController(PatientOTPService patientOTPService)
        {
            this.patientOTPService = patientOTPService;
        }
        [Authorize]
        [HttpPost]
        [Route("generate-otp")]
        public async Task<IActionResult> GenerateOTP([FromBody] GeneratePatientOTPRequestModel requestModel)
        {
            var result = await patientOTPService.GenerateOTPAsync(requestModel);

            if (result == null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Failed to generate OTP"
                });
            }

            return Ok(new
            {
                Status = true,
                Message = "OTP generated successfully",
                Data = result
            });
        }

        [Authorize]
        [HttpPost]
        [Route("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyPatientOTPRequestModel requestModel)
        {
            var result = await patientOTPService.VerifyOTPAsync(requestModel);
            if (result == null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "OTP Verification Failed"
                });
            }

            return Ok(result);
        }
    }
}
