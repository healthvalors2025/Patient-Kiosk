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
    public class DoctorController : ControllerBase
    {
        private readonly DoctorService _doctorService;
        private readonly IDbConnectionFactory _db;
        private readonly IConfiguration _configuration;

        public DoctorController(DoctorService doctorService, IDbConnectionFactory db,IConfiguration configuration)
        {
            _doctorService = doctorService;
            _db = db;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("get-doctor-list")]
        public async Task<IActionResult> GetDoctorList([FromQuery] DoctorRequestModel requestModel)
        {
            var doctorList = await _doctorService.GetDoctorListAsync(requestModel);
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
    }
}
