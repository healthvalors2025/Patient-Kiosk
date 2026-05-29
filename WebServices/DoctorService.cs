using DoctorMobileApp.CommonClass;
using Microsoft.Data.SqlClient;
using PatientKiosk.Models;
using System.Collections.Generic;
using System.Data;

namespace PatientKiosk.WebServices
{
    public class DoctorService
    {
        private readonly IDbConnectionFactory _dbHelper;
        private readonly IConfiguration _configuration;

        public DoctorService(IDbConnectionFactory db, IConfiguration configuration)
        {
            _dbHelper = db;
            _configuration = configuration;
        }

       public async Task<List<DoctorResponseModel>> GetDoctorListAsync(DoctorRequestModel requestModel)
        {
            var list = new List<DoctorResponseModel>();
            var doctorParams = new[]
            {
                new SqlParameter("@HospitalIDF", requestModel.HospitalIDF),
                new SqlParameter("@SkillSetIDF", requestModel.SkillSetIDF)
            };
            list = await _dbHelper.QueryAsync<DoctorResponseModel>("Kiosk_API_Doctor_GetList", CommandType.StoredProcedure, doctorParams);
            return list;
        }

    }
}
