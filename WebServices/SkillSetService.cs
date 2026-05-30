using DoctorMobileApp.CommonClass;
using Microsoft.Data.SqlClient;
using PatientKiosk.Models;
using System.Data;
namespace PatientKiosk.WebServices
{
    public class SkillSetService
    {
        private readonly IDbConnectionFactory _dbHelper;
        private readonly IConfiguration _configuration;
        public SkillSetService(IDbConnectionFactory db, IConfiguration configuration)
        {
            _dbHelper = db;
            _configuration = configuration;
        }

        public async Task<List<SkillSetResponseModel>> GetSkillSetListAsync(SkillSetRequestModel requestModel)
        {
            var list = new List<SkillSetResponseModel>();
            var skillSetParams = new[]
            {
                new SqlParameter("@HospitalGroupIDF", requestModel.HospitalGroupIDF)
            };
            list = await _dbHelper.QueryAsync<SkillSetResponseModel>("API_SP_GetStandardSkillSetList", CommandType.StoredProcedure, skillSetParams);
            return list;
        }
    }
}
