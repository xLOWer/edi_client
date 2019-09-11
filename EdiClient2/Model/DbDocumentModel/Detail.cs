using System;

namespace EdiClient.Model
{
    public class Detail
    {

        public string ID { get; set; }
        public string ID_GOOD { get; set; }
        public string ID_EDI_DOC { get; set; }
        public string EAN { get; set; }
        public string SUPPLIER_ITEM_CODE { get; set; }
        public string BUYER_ITEM_CODE { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public string ORDERED_QUANTITY { get; set; }
        public string TAX_RATE { get; set; }
        public string ORDERED_GROSS_AMOUNT { get; set; }
        public string ORDERED_TAX_AMOUNT { get; set; }
        public string ORDERED_NET_AMOUNT { get; set; }
        public string ORDERED_UNIT_NET_PRICE { get; set; }
        public string ORDERED_UNIT_GROSS_PRICE { get; set; }
        public string GROSS_WEIGHT { get; set; }
        public string NET_WEIGHT { get; set; }
        public string EXPIRATION_DATE { get; set; }
        public string MAX_EXPIRATION_DATE { get; set; }
        public string REASON_CODE { get; set; }
        public string LINE_NUMBER { get; set; }
        public string QUANTITY { get; set; }
        public string PRICE { get; set; }

        public string UnitsDifference { get; set; }

        public string UnitGrossPrice { get; set; }
        public string GrossAmount { get; set; }
        public string TaxAmount { get; set; }
        public string NetAmount { get; set; }

        public bool IsFailed => string.IsNullOrEmpty(ID_GOOD) || ID_GOOD=="0";
        public bool IsNotmatched => QUANTITY is null || GrossAmount is null;
        public bool HasDiffrence => double.Parse(QUANTITY) != double.Parse(ORDERED_QUANTITY) || double.Parse(PRICE) != double.Parse(ORDERED_UNIT_GROSS_PRICE);
        
    }
}