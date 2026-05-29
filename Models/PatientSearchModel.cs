namespace PatientKiosk.Models
{
    public class PatientSearchModel
    {
        public string? MobileNo { get; set; }
        public string? ABHANo { get; set; }
        public string? CRNo { get; set; }
        public int HospitalIDF {  get; set; }
    }
    public class PatientDetail
    {
        public int PatientID { get; set; }
        public string? PatientName { get; set; }
        public string? ClassName { get; set; }
        public string? MobileNo { get; set; }
        public string? ABHANo { get; set; }
        public string? CRNo { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
    }
}
