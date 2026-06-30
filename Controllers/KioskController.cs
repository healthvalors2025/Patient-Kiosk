using DoctorMobileApp.CommonClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PatientKiosk.Models;
using PatientKiosk.WebServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static PatientKiosk.Models.KioskModel;
namespace PatientKiosk.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class KioskController : ControllerBase
    {
        private readonly KioskService _kioskService;
        private readonly IDbConnectionFactory _db;
        private readonly IConfiguration _configuration;
        private const string StaticUsername = "Kiosk";
        private const string StaticPassword = "Kiosk@123";
        private readonly ILogger _logger;
        private int hospitalidf => int.TryParse(User.FindFirst("HospitalIDF")?.Value, out var id) ? id : 0;
        private int hospitalgroupidf => int.TryParse(User.FindFirst("HospitalGroupIDF")?.Value, out var id) ? id : 0;
        private int UserIdf => int.TryParse(User.FindFirst("UserIdf")?.Value, out var id) ? id : 0;
        public KioskController(KioskService kioskService, IDbConnectionFactory db, IConfiguration configuration, ILogger logger)
        {
            _kioskService = kioskService;
            _db = db;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [Route("get-patient-detail")]
        public async Task<IActionResult> GetPatientSearchDeatil([FromBody] PatientSearchModel patientSearch)
        {
            var patientDetail = await _kioskService.GetPatientSearchListAsync(patientSearch);
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

        [HttpPost]
        [Route("get-skill-set")]
        public async Task<IActionResult> GetSkillSet([FromBody] SkillSetRequestModel requestModel)
        {
            var skillSetList = await _kioskService.GetSkillSetListAsync(requestModel);
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

        [HttpPost]
        [Route("generate-otp")]
        public async Task<IActionResult> GenerateOTP([FromBody] GeneratePatientOTPRequestModel requestModel)
        {
            var result = await _kioskService.GenerateOTPAsync(requestModel);

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

        [HttpPost]
        [Route("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyPatientOTPRequestModel requestModel)
        {
            var result = await _kioskService.VerifyOTPAsync(requestModel);
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

        [HttpPost]
        [Route("get-patho-report-list-for-print")]
        public async Task<IActionResult> GetPathoReportListForPrint([FromBody] PathoReportRequestModel requestModel)
        {
            var pathoReportDetail = await _kioskService.GetPathoReportListForPrintAsync(requestModel);
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

        [HttpPost]
        [Route("get-opd-test-receipt")]
        public async Task<IActionResult> GetOPDTestReceipt([FromBody] OPDTestReceiptRequestModel requestModel)
        {
            var opdTestReceiptList = await _kioskService.GetOPDTestReceiptListAsync(requestModel);
            if (opdTestReceiptList == null || opdTestReceiptList.Count == 0)
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
                Data = opdTestReceiptList
            });
        }

        [HttpPost]
        [Route("save-opd-test-receipt")]
        public async Task<IActionResult> SaveOPDTestReceipt([FromBody] SaveOPDTestReceiptRequestModel receiptModel)
        {
            if (receiptModel == null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid Request"
                });
            }
            var receiptId = await _kioskService.SaveOPDTestReceiptAsync(receiptModel);
            if (receiptId <= 0)
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
                Message = "OPD Test Receipt Saved Successfully",
                ReceiptID = receiptId
            });
        }

        [HttpPost]
        [Route("get-doctor-list")]
        public async Task<IActionResult> GetDoctorList([FromQuery] DoctorRequestModel requestModel)
        {
            var doctorList = await _kioskService.GetDoctorListAsync(requestModel);
            if (doctorList == null || doctorList.Count == 0)
            {
                return NotFound(new
                {
                    Status = false,
                    Message = "Doctor Not Found"
                });
            }
            return Ok(new
            {
                Status = true,
                Message = "Success",
                Data = doctorList
            });
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
            var voucherId = await _kioskService.SaveAdvanceDepositAsync(depositmodel);
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

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate static credentials
            if (!string.Equals(request.Username, StaticUsername, StringComparison.Ordinal) || !string.Equals(request.Password, StaticPassword, StringComparison.Ordinal))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Read JWT settings from configuration
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiryHoursValue = _configuration["Jwt:ExpiryHours"];

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                // Misconfiguration should be discovered during deployment; return 500 to indicate server config issue
                return StatusCode(500, new { message = "JWT configuration is invalid" });
            }

            if (!int.TryParse(expiryHoursValue, out var expiryHours))
            {
                expiryHours = 24; // fallback
            }

            var expiresAt = DateTime.UtcNow.AddHours(expiryHours);
            var claims = new List<Claim>
            {
                new Claim("username", request.Username)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(tokenDescriptor);
            var response = new LoginResponse
            {
                Token = tokenString,
                ExpiresAtUtc = expiresAt,
                Username = request.Username
            };
            return Ok(response);
        }
    }
}
