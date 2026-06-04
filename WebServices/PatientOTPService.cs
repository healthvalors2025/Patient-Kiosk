using DoctorMobileApp.CommonClass;
using Microsoft.Data.SqlClient;
using PatientKiosk.Models;
using System.Data;

namespace PatientKiosk.WebServices
{
    public class PatientOTPService
    {
        private readonly IDbConnectionFactory _dbHelper;
        private readonly IConfiguration _configuration;

        public PatientOTPService(IDbConnectionFactory db, IConfiguration configuration)
        {
            _dbHelper = db;
            _configuration = configuration;
        }

        public async Task<GeneratePatientOTPResponseModel> GenerateOTPAsync(GeneratePatientOTPRequestModel requestModel)
        {
            var otpParams = new[]
            {
                new SqlParameter("@PatientIDF", requestModel.PatientIDF),
                new SqlParameter("@CRNumber", requestModel.CRNumber),
                new SqlParameter("@MobileNo", requestModel.MobileNo),
                new SqlParameter("@HospitalIDF", requestModel.HospitalIDF)
            };
            var result = await _dbHelper.QueryAsync<GeneratePatientOTPResponseModel>("Kiosk_API_GeneratePatientOTP", CommandType.StoredProcedure, otpParams);
            return result.FirstOrDefault();
        }
        public async Task<VerifyPatientOTPResponseModel> VerifyOTPAsync(VerifyPatientOTPRequestModel requestModel)
        {
            var otpParams = new[]
            {
                    new SqlParameter("@KioskPatientOTPIDP", requestModel.KioskPatientOTPIDP),
                    new SqlParameter("@CRNumber", requestModel.CRNumber),
                    new SqlParameter("@KioskOTP", requestModel.KioskOTP)
            };
            var result = await _dbHelper.QueryAsync<VerifyPatientOTPResponseModel>("Kiosk_API_VerifyPatientOTP", CommandType.StoredProcedure,otpParams);
            return result.FirstOrDefault();
        }
    }
}