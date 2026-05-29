using DoctorMobileApp.CommonClass;
using Microsoft.Data.SqlClient;
using PatientKiosk.Models;
using System.Data;

namespace PatientKiosk.WebServices
{
    public class OPDTestReceiptService
    {
            private readonly IDbConnectionFactory _dbHelper;
            private readonly IConfiguration _configuration;
            public OPDTestReceiptService(IDbConnectionFactory db, IConfiguration configuration)
            {
                _dbHelper = db;
                _configuration = configuration;
        }

        public async Task<List<OPDTestReceiptResponseModel>> GetOPDTestReceiptListAsync(OPDTestReceiptRequestModel requestModel)
        {
            var list = new List<OPDTestReceiptResponseModel>();
            var OPDParams = new[]
            {
                
                new SqlParameter("@PatientIDF" , requestModel.PatientIDF),
                new SqlParameter("@HospitalIDF", requestModel.HospitalIDF)
            };
            list = await _dbHelper.QueryAsync<OPDTestReceiptResponseModel>("Kiosk_API_OPDTestReceipt_GetList", CommandType.StoredProcedure, OPDParams);
            return list;
        }
    }
}
