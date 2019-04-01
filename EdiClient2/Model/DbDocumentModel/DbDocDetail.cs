namespace EdiClient.Model
{
    class DbDocDetail
    {
        public string ID_DOC { get; set; }
        public string ID_GOOD { get; set; }
        public string QUANTITY { get; set; }
        public string PRICE { get; set; }
        public string ID_ITEM { get; set; }
        public string ITEM_QUANTITY { get; set; }
        public string ITEM_POS { get; set; }
        public string DISCOUNT_RATE { get; set; }
        public string DISCOUNT_SUMM { get; set; }
        public string CHARGE_RATE { get; set; }
        public string CHARGE_SUMM { get; set; }
        public string LOCK_STATUS { get; set; }
        public string ERR { get; set; }
        public string PRIME { get; set; }
        public string SERT_NUM { get; set; }
        public string REG_NUM { get; set; }
        //public string EXPIRING_DATE { get; set; }
        public string CODE { get; set; }
        public string TAX { get; set; }
        public string ID_BASE_ITEM { get; set; }
        public string ID_DEFAULT_ITEM { get; set; }
        public string ID_ACCOUNT_CURRENCY { get; set; }
        public string ID_MANUFACTURER { get; set; }
        public string ID_ORGAN { get; set; }
        public string ID_COUNTRY { get; set; }
        public string CUSTOMS_NO { get; set; }
        public string ID_SUBDIVISION { get; set; }
        public string HAS_REMAIN { get; set; }
        public string OLDID { get; set; }
        public string GOOD_SIZE { get; set; }
        public string BAR_CODE { get; set; }
        public string ID_EDI_DOC { get; set; }
        public string LineNumber { get; set; }
        public string BuyerItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string OrderedUnitNetPrice { get; set; }
        public string OrderedUnitGrossPrice { get; set; }
        public string TaxRate { get; set; }
        public string ReasonCode { get; set; }
        public string EAN { get; set; }
        //public string ExpirationDate { get; set; }
        public string GrossWeight { get; set; }
        //public string MaxExpirationDate { get; set; }
        public string NetWeight { get; set; }
        public string OrderedGrossAmount { get; set; }
        public string OrderedNetAmount { get; set; }
        public string OrderedPallets { get; set; }
        public string OrderedTaxAmount { get; set; }
        public string OrderedUnitPacksize { get; set; }
        public string SupplierItemCode { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Volume { get; set; }
        public string OrderedBoxes { get; set; }
        public string OrderedQuantity { get; set; }
        public string Failed { get; set; }
    }
}