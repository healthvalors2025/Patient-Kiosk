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
        public async Task<int> SaveOPDTestReceiptAsync(SaveOPDTestReceiptRequestModel model)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("InvestigationRegistrationIDP", typeof(int));
                dt.Columns.Add("InvestigationType", typeof(int));
                dt.Columns.Add("Rate", typeof(decimal));
            
                foreach (var item in model.OPDTestReceiptList)
                {
                    dt.Rows.Add(
                        item.InvestigationRegistrationIDP,
                        item.InvestigationType,
                        item.Rate
                    );
                }
                var tvpParam = new SqlParameter
                {
                    ParameterName = "@KioskOPDTestReceiptTableType",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.KioskOPDTestReceiptTableType",
                    Value = dt
                };
                var voucherParam = new SqlParameter("@VoucherIDP_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                var parameters = new SqlParameter[]
                {
                   new SqlParameter("@HospitalIDF", model.HospitalIDF),
                   new SqlParameter("@PatientIDF", model.PatientIDF),
                   new SqlParameter("@OPDRegistrationIDF", model.OPDRegistrationIDF),
                   tvpParam,
                   new SqlParameter("@UserIDF", model.UserIDF),
                   new SqlParameter("@UPITransactionNo",
                   string.IsNullOrEmpty(model.UPITransactionNo)? DBNull.Value: (object)model.UPITransactionNo),
                   voucherParam
                };
                await _dbHelper.ExecuteNonQueryAsync("Kiosk_API_OPDTestReceipt_Save", CommandType.StoredProcedure,parameters);
                return Convert.ToInt32(voucherParam.Value);
            }
            catch( Exception ex) 
            {
                return 0;
            }
        }
    }
}
