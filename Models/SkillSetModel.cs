namespace PatientKiosk.Models
{
    public class SkillSetRequestModel
    {
        public int HospitalGroupIDF { get; set; }
    }
    public class SkillSetResponseModel
    {
        public int SkillSetIDF { get; set; }
        public string? SkillSetName { get; set; }
        public string? SkillSetLocalLanguage { get; set; }
        public int? MobileDoctorSkillIDP { get; set; }
        public string? IconPath { get; set; }
        public int? HospitalGroupIDF { get; set; }
    }
}
//public string? EntryDate { get; set; }
