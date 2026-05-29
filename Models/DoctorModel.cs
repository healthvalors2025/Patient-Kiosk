namespace PatientKiosk.Models
{
    public class DoctorRequestModel
    {
        public int HospitalIDF { get; set; }
        public int SkillSetIDF { get; set; }

    }

    public class DoctorResponseModel
    {
        public int DoctorID { get; set; }
        public string? DoctorName { get; set; }
    }
    public class DoctorDetailsResponseModel
    {
        public int DoctorID { get; set; }
        public string? DoctorName { get; set; }
        public string? HospitalName { get; set; }
        public string? SkillSet { get; set; }
    }
}
