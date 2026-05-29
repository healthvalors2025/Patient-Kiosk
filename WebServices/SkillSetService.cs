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
                new SqlParameter("@SkillSetID", requestModel.SkillSetID),
                new SqlParameter("@SkillSetName", requestModel.SkillSetName ?? (object)DBNull.Value)
            };
            list = await _dbHelper.QueryAsync<SkillSetResponseModel>("Kiosk_API_SkillSet_GetList", CommandType.StoredProcedure, skillSetParams);
            return list;
        }
    }
}
