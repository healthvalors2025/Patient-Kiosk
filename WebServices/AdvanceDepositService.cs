using DoctorMobileApp.CommonClass;
using Microsoft.Data.SqlClient;
using PatientKiosk.Models;
using System;
using System.Data;


namespace PatientKiosk.WebServices
{
    public class AdvanceDepositService
    {
        private readonly IDbConnectionFactory _dbHelper;
        private readonly IConfiguration _configuration;
        public AdvanceDepositService(IDbConnectionFactory db, IConfiguration configuration)
        {
            _dbHelper = db;
            _configuration = configuration;
        }
        public async Task<int> SaveAdvanceDepositAsync(AdvanceDepositModel model)
        {
            try
            {
                var voucherParam = new SqlParameter("@VoucherIDP", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var parameters = new[]
                {
                     new SqlParameter("@PatientIDF", model.PatientIDF),
                     new SqlParameter("@AdvanceAmount", model.AdvanceAmount),
                     new SqlParameter("@TransactionId",string.IsNullOrEmpty(model.TransactionId)? DBNull.Value: (object)model.TransactionId),
                     new SqlParameter("@HospitalIDF", model.HospitalIDF),
                     new SqlParameter("@ModeOfPaymentIDF", model.ModeOfPaymentIDF),
                     new SqlParameter("@Kiosk_UserIDF", model.Kiosk_UserIDF),
                     voucherParam
                };
                await _dbHelper.ExecuteNonQueryAsync("Kiosk_API_InsertPatientAdvance",CommandType.StoredProcedure,parameters);
                return Convert.ToInt32(voucherParam.Value);
            }
            catch
            {
                return 0;
            }
        }
    }
}

