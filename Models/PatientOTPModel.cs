namespace PatientKiosk.Models
{
    public class    GeneratePatientOTPRequestModel
    {
        public int PatientIDF { get; set; }
        public string? CRNumber { get; set; }
        public string? MobileNo { get; set; }
        public int HospitalIDF { get; set; }
    }
    public class GeneratePatientOTPResponseModel
    {
        public long OTPIDP { get; set; }
        public string? CRNumber { get; set; }
        public string? MobileNo { get; set; }
        public string? Message { get; set; }
        public string? OTP { get; set; }
    }
    public class VerifyPatientOTPRequestModel
    {
        public long KioskPatientOTPIDP { get; set; }
        public string? CRNumber { get; set; }
        public string? KioskOTP { get; set; }
    }
    public class VerifyPatientOTPResponseModel
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
    }
}
