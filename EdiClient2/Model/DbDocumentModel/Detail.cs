using System;

namespace EdiClient.Model
{
    public class Detail
    {
        public string ID_GOOD { get; set; }
        public string ID_EDI_DOC { get; set; }
        public string EAN { get; set; }
        public string SUPPLIER_ITEM_CODE { get; set; }
        public string BUYER_ITEM_CODE { get; set; }
        public string FAILED { get; set; }
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

        public bool IsFailed => string.IsNullOrEmpty(FAILED) ? false : true;
                
        public bool HasDiffrence => UnitsCount != double.Parse(ORDERED_QUANTITY) || UnitNetPrice != double.Parse(ORDERED_UNIT_GROSS_PRICE) ? true : false;

        public double TaxRate => double.Parse(TAX_RATE);
        public double UnitNetPrice => double.Parse(PRICE);
        public double UnitsCount => double.Parse(QUANTITY);
        public double UnitsDifference => double.Parse(ORDERED_QUANTITY) - UnitsCount;

        public double UnitGrossPrice => Math.Round(UnitNetPrice / 100 * (100 + TaxRate), 2);
        public double GrossAmount => Math.Round(UnitGrossPrice * UnitsCount, 2);
        public double TaxAmount => Math.Round(GrossAmount * TaxRate / (100 + TaxRate), 2);
        public double NetAmount => GrossAmount - TaxAmount;

    }
}