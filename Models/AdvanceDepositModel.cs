using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientKiosk.Models
{
    public class AdvanceDepositModel
    {
        public int PatientIDF { get; set; }
        public int HospitalIDF { get; set; }
        public decimal AdvanceAmount { get; set; }
        public string? TransactionId { get; set; }
        public int ModeOfPaymentIDF { get; set; }
        public int Kiosk_UserIDF { get; set; }
    }
    //public class AdvanceDepositResponseModel
    //{
    //    public int? PatientIDF { get; set; }
    //    public int? HospitalIDF { get; set; }
    //    public decimal? AdvanceAmount { get; set; }
    //    public string? TransactionId { get; set; }
    //    public int? ModeOfPaymentIDF { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //}
}
