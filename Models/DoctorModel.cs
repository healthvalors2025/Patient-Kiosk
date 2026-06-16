namespace PatientKiosk.Models
{
    public class DoctorRequestModel
    {
        public int HospitalIDF { get; set; }
        public int SkillSetIDF { get; set; }
        //Test
    }
    public class DoctorResponseModel
    {
        public int DoctorID { get; set; }
        public string? DoctorName { get; set; }
    }
}
