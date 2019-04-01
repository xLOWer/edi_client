namespace EdiClient.Model
{
    class DbDocHeader
    {
        public string ID { get; set; }
        public string ID_CUSTOMER { get; set; }
        public string DOC_TYPE { get; set; }
        public string ID_TRADER { get; set; }
        public string SenderILN { get; set; }
        public string PromotionReference { get; set; }
        public string ExpectedDeliveryDate { get; set; }
        public string PaymentDueDate { get; set; }
        public string DeliveryPointIln { get; set; }
        public string BuyerIln { get; set; }
        public string CustomerIln { get; set; }
        public string SellerIln { get; set; }
        public string OrderCurrency { get; set; }
        public string OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string OrderPaymentDueDate { get; set; }
        public string Remarks { get; set; }
        public string TotalGrossAmount { get; set; }
        public string IS_IN_EDI_AS_ORDRSP { get; set; }
        public string IS_IN_EDI_AS_DESADV { get; set; }
        public string CODE { get; set; }
        public string COMMENTS { get; set; }
        public string DOC_DATETIME { get; set; }
        public string ID_DOC_TYPE { get; set; }
        public string ID_INSTANCE_SENDER { get; set; }
        public string ID_INSTANCE_RECIEPIENT { get; set; }
        public string ID_INSTANCE_OWNER { get; set; }
        public string ID_DOC_MASTER { get; set; }
        public string ERROR_STATUS { get; set; }
        public string ACT_STATUS { get; set; }
        public string LOCK_STATUS { get; set; }
        public string USER_NAME { get; set; }
        public string CREATE_INVOICE { get; set; }
        public string DELIVERY_DATE { get; set; }
        public string DELETED { get; set; }
        public string UPDATED_PRICE { get; set; }
        public string PAY_DELAY { get; set; }
        public string ID_AGENT { get; set; }
        public string ID_CURRENCY { get; set; }
        public string CURRENCY_RATE { get; set; }
        public string ID_PRICE_TYPE { get; set; }
        public string DISCOUNT_RATE { get; set; }
        public string DISCOUNT_SUMM { get; set; }
        public string TOTAL_SUMM { get; set; }
        public string ID_SELLER { get; set; }
        public string ID_CUSTOMER1 { get; set; }
        public string ID_STORE_SENDER { get; set; }
        public string ID_STORE_RECIEPIENT { get; set; }
        public string ID_SUBDIVISION { get; set; }
        public string ID_DOC_RETURN { get; set; }
        public string CHARGE_RATE { get; set; }
        public string CHARGE_SUMM { get; set; }
        public string IS_RETURN { get; set; }
        public string LOCK_STATUS1 { get; set; }
        public string DOC_PRECISION { get; set; }
        public string TOTAL_PRIME { get; set; }
        public string TOTAL_LINES { get; set; }
    }
}