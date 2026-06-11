namespace PatientKiosk.Models
{
    public class OPDTestReceiptRequestModel
    {
        public int PatientIDF { get; set; }
        public int HospitalIDF { get; set; }
    }

    public class OPDTestReceiptResponseModel
    {
        public int OPDRegistrationIDP { get; set; }
        public int InvestigationRegistrationIDP { get; set; }
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
    public class SaveOPDTestReceiptRequestModel
    {
        public int HospitalIDF { get; set; }
        public int PatientIDF { get; set; }
        public int OPDRegistrationIDF { get; set; }
        public int UserIDF { get; set; }
        public string? UPITransactionNo { get; set; }
        public List<OPDTestReceiptDetailModel> OPDTestReceiptList { get; set; } = new List<OPDTestReceiptDetailModel>();
    }
    public class OPDTestReceiptDetailModel
    {
        public int InvestigationRegistrationIDP { get; set; }
        public int InvestigationType { get; set; }
        public decimal Rate { get; set; }
    }
    public class SaveOPDTestReceiptResponseModel
    {
        public int VoucherIDP { get; set; }
    }
}