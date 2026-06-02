using DoctorMobileApp.CommonClass;
using Microsoft.Data.SqlClient;
using PatientKiosk.Models;
using System.Data;
using Microsoft.AspNetCore.Http;
namespace PatientKiosk.WebServices
{
    public class SkillSetService
    {
        private readonly IDbConnectionFactory _dbHelper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SkillSetService(IDbConnectionFactory db, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbHelper = db;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<SkillSetResponseModel>> GetSkillSetListAsync(SkillSetRequestModel requestModel)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            string baseUrl = $"{request.Scheme}://{request.Host}";

            var list = new List<SkillSetResponseModel>();

            var skillSetParams = new[]
            {
                new SqlParameter("@HospitalGroupIDF", requestModel.HospitalGroupIDF)
            };

            list = await _dbHelper.QueryAsync<SkillSetResponseModel>("API_SP_GetStandardSkillSetList",CommandType.StoredProcedure,skillSetParams);
            
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.IconPath))
                {
                    string[] pathParts = item.IconPath.Split('\\');
                    string hospitalCode = "";
                    if (pathParts.Length > 1)
                    {
                        hospitalCode = pathParts[2];
                    }
                    int index = item.IconPath.IndexOf("MobileApp", StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                    {
                        string relativePath = item.IconPath.Substring(index).Replace("\\", "/");
                        item.IconPath = $"{baseUrl}/{hospitalCode}/{relativePath}";
                    }
                }
            }
            return list;
        }
    }
}
//foreach (var item in list)
//{
//    if (!string.IsNullOrEmpty(item.IconPath))
//    {
//        int index = item.IconPath.IndexOf("MobileApp", StringComparison.OrdinalIgnoreCase);

//        if (index >= 0)
//        {
//            item.IconPath = item.IconPath.Substring(index);
//        }
//    }
//}