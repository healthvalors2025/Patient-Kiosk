namespace PatientKiosk.Models
{
    public class PathoReportRequestModel
    {
        public int PatientIDF { get; set; }
        public int HospitalIDF { get; set; }
      
    }
    public class PathoReportResponseModel
    {
        public int PathoRegistrationIDP { get; set; }
        public string? ReportName { get; set; }
        public string? RegistrationCode { get; set; }
        public DateTime RegistrationDateTime { get; set; }
        public string? ReportStatusDisplay { get; set; }
        public bool ReportStatus { get; set; }
        public string? Doctor { get; set; }
    }
}
