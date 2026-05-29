using DoctorMobileApp.CommonClass;
using Microsoft.Data.SqlClient;
using PatientKiosk.Models;
using System.Data;

namespace PatientKiosk.WebServices
{
    public class PathoReportService
    {
        private readonly IDbConnectionFactory _dbHelper;
        private readonly IConfiguration _configuration;

        public PathoReportService(IDbConnectionFactory db, IConfiguration configuration)
        {
            _dbHelper = db;
            _configuration = configuration;
        }
         public async Task<List<PathoReportResponseModel>> GetPathoReportListAsync(PathoReportRequestModel requestModel)
        {
            var list = new List<PathoReportResponseModel>();
            var pathoReportParams = new[]
            {
                 new SqlParameter("@HospitalIDF", requestModel.HospitalIDF)
            };
            list = await _dbHelper.QueryAsync<PathoReportResponseModel>("Kiosk_API_PathologyReport_GetList", CommandType.StoredProcedure, pathoReportParams);
            return list;
        }
    }
}
