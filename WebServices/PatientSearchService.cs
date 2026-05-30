using DoctorMobileApp.CommonClass;
using Microsoft.Data.SqlClient;
using PatientKiosk.Models;
using System.Data;

namespace PatientKiosk.WebServices
{
    public class PatientSearchService
    {
        private readonly IDbConnectionFactory _dbHelper;
        private readonly IConfiguration _configuration;
        public PatientSearchService(IDbConnectionFactory db, IConfiguration configuration)
        {
            _dbHelper = db;
            _configuration = configuration;
        }
        public async Task<List<PatientDetail>> GetPatientSearchListAsync(PatientSearchModel searchModel)
        {
            var list = new List<PatientDetail>();
            var patientParams = new[]
            {
                new SqlParameter("@MobileNo", searchModel.MobileNo ?? (object)DBNull.Value),
                new SqlParameter("@ABHANo", searchModel.ABHANo ?? (object)DBNull.Value),
                new SqlParameter("@CRNo", searchModel.CRNo ?? (object)DBNull.Value),
                new SqlParameter("@HospitalIDF", searchModel.HospitalIDF)
            };
            list = await _dbHelper.QueryAsync<PatientDetail>("Kiosk_API_PatientSearch", CommandType.StoredProcedure,patientParams);
            return list;
        }   
    }
}
