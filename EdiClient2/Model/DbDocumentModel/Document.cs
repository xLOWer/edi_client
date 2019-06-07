using System.Collections.Generic;

namespace EdiClient.Model
{
    public class Document
    {
        public string ID { get; set; }
        public string ID_CUSTOMER { get; set; }
        public string ID_TRADER { get; set; }
        public string ORDER_NUMBER { get; set; }
        public string ORDER_DATE { get; set; }
        public string ORDER { get; set; }
        public string ORDRSP { get; set; }
        public string DESADV { get; set; }
        public string FAILED { get; set; }
        public string TOTAL_GROSS_AMOUNT { get; set; }
        public string SENDER_ILN { get; set; }
        public string DELIVERY_POINT_ILN { get; set; }
        public string BUYER_ILN { get; set; }
        public string CUSTOMER_ILN { get; set; }
        public string SELLER_ILN { get; set; }
        public string ORDER_CURRENCY { get; set; }
        public string EXPECTED_DELIVERY_DATE { get; set; }
        public string REMARKS { get; set; }
        public string CREATOR { get; set; }
        public string CODE { get; set; }
        public string DOC_DATETIME { get; set; }
        public string ACT_STATUS { get; set; }

        public bool IsFailed => FAILED == "1";
        public bool IsReadyToTrader => string.IsNullOrEmpty(ID_TRADER) && !IsOrdrsp && !IsDesadv && !IsFailed;
        public bool IsReadyToOrdrsp => !string.IsNullOrEmpty(ID_TRADER) && ACT_STATUS == "3" && !IsFailed;
        public bool IsReadyToDesadv => !string.IsNullOrEmpty(ORDRSP) && ACT_STATUS == "4" && !IsFailed;
        public bool IsInTrader => !string.IsNullOrEmpty(ID_TRADER) && !IsFailed;
        public bool IsOrdrsp => !string.IsNullOrEmpty(ORDRSP) && !IsFailed;
        public bool IsDesadv => !string.IsNullOrEmpty(DESADV) && !IsFailed;
        //public bool IsRecadv => !string.IsNullOrEmpty(RECADV) ? true : false;

        public List<Detail> Details { get; set; }
    }
}