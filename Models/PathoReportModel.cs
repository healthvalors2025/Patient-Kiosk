namespace PatientKiosk.Models
{
    public class PathoReportRequestModel
    {
        public int HospitalIDF { get; set; }

    }
    public class PathoReportResponseModel
    {
        public int ReportMasterID { get; set; }
        public string? ReportName { get; set; }
        public int ServiceIDF { get; set; }
        public string? ServiceName { get; set; }
        public bool Paid { get; set; }
        public int PriceListIDP { get; set; }
        public decimal Rate { get; set; }
        /*
            1 = Pathology
            2 = Radiology
            3 = Medical Procedure
        */
        public int InvestigationType { get; set; }
        public string? Doctor { get; set; }
    }
}
