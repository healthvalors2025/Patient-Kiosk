using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientKiosk.Models;
using PatientKiosk.WebServices;

namespace PatientKiosk.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OPDTestReceiptController : ControllerBase
    {
        private readonly OPDTestReceiptService _OPDTestReceiptService;

        public OPDTestReceiptController(OPDTestReceiptService OPDTestReceiptService)
        {
            _OPDTestReceiptService = OPDTestReceiptService;
        }
        [Authorize]
        [HttpPost]
        [Route("get-opd-test-receipt")]
        public async Task<IActionResult> GetOPDTestReceipt([FromBody] OPDTestReceiptRequestModel requestModel)
        {
            var opdTestReceiptList = await _OPDTestReceiptService.GetOPDTestReceiptListAsync(requestModel);
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
        
        [Authorize]
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
            var receiptId = await _OPDTestReceiptService.SaveOPDTestReceiptAsync(receiptModel);
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
    }
}
