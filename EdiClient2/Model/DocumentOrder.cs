using System;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace EdiClient.Model
{
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("Document-Order", Namespace = "", IsNullable = false)]
    public partial class DocumentOrder : INotifyPropertyChanged
    {
        [XmlElement("Document-Header")]
        public DocumentOrderDocumentHeader DocumentHeader { get; set; }
        [XmlElement("Order-Header")]
        public DocumentOrderOrderHeader OrderHeader { get; set; }
        [XmlElement("Document-Parties")]
        public DocumentOrderDocumentParties DocumentParties { get; set; }
        public DocumentOrderDetailsOfTransport DetailsOfTransport { get; set; }
        [XmlElement("Order-Parties")]
        public DocumentOrderOrderParties OrderParties { get; set; }
        [XmlElement("Order-Lines")]
        public DocumentOrderOrderLines OrderLines { get; set; }
        [XmlElement("Order-Summary")]
        public DocumentOrderOrderSummary OrderSummary { get; set; }
        [XmlElement("Document-Attachments")]
        public DocumentOrderDocumentAttachments DocumentAttachments { get; set; }

        [XmlIgnore]
        private string TraderNumber;
        [XmlIgnore]
        private bool isInDatabase;
        [XmlIgnore]
        public bool IsInDatabase
        {
            get { return isInDatabase; }
            set
            {
                isInDatabase = value;
                NotifyPropertyChanged("IsInDatabase");
            }
        }
        [XmlIgnore]
        private bool isFailed;
        [XmlIgnore]
        public bool IsFailed
        {
            get { return isFailed; }
            set
            {
                isFailed = value;
                NotifyPropertyChanged("IsFailed");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderDocumentHeader
    {
        public DateTime DocumentReceiveDateTime { get; set; }
        public DateTime DocumentProcessDateTime { get; set; }
        public string DocumentID { get; set; }
        public string DocumentUID { get; set; }
        public string DocumentLink { get; set; }
        public string DocumentVersion { get; set; }
        public string DocumentType { get; set; }
        public string DocumentSource { get; set; }
        public string OriginalFileName { get; set; }
        public string SenderMessageID { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderHeader
    {
        public string OrderNumber { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime OrderDate { get; set; }
        public string OrderCurrency { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime ExpectedDeliveryDate { get; set; }
        public string ExpectedDeliveryTime { get; set; }
        public string PromotionReference { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime OrderPaymentDueDate { get; set; }
        public string OrderPaymentTerms { get; set; }
        public string DocumentFunctionCode { get; set; }
        public string DocumentNameCode { get; set; }
        public string Remarks { get; set; }
        public string SpecialInstructions { get; set; }
        [XmlElement("Order-Transport")]
        public DocumentOrderOrderHeaderOrderTransport OrderTransport { get; set; }
        public DocumentOrderOrderHeaderReference Reference { get; set; }
        public DocumentOrderOrderHeaderAdditionalData AdditionalData { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderHeaderOrderTransport
    {
        public string TermsOfDelivery { get; set; }
        public string ConveyanceReferenceNumber { get; set; }
        public string ModeOfTransport { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderHeaderReference
    {
        [XmlElement(DataType = "date")]
        public DateTime OrderReferenceDate { get; set; }
        public string OrderReferenceNumber { get; set; }
        public string ContractNumber { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime ContractDate { get; set; }
        [XmlElement("Reference-Elements")]
        public DocumentOrderOrderHeaderReferenceReferenceElements ReferenceElements { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderHeaderReferenceReferenceElements
    {
        [XmlElement("Reference-Element")]
        public DocumentOrderOrderHeaderReferenceReferenceElementsReferenceElement ReferenceElement { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderHeaderReferenceReferenceElementsReferenceElement
    {
        [XmlElement("Reference-Type")]
        public string ReferenceType { get; set; }
        [XmlElement("Reference-Id")]
        public string ReferenceId { get; set; }
        [XmlElement("Reference-Date", DataType = "date")]
        public DateTime ReferenceDate { get; set; }
        public DocumentOrderOrderHeaderReferenceReferenceElementsReferenceElementAdditionalData AdditionalData { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderHeaderReferenceReferenceElementsReferenceElementAdditionalData
    {
        public string Content { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderHeaderAdditionalData
    {
        public string Content { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderDocumentParties
    {
        public DocumentOrderDocumentPartiesSender Sender { get; set; }
        public DocumentOrderDocumentPartiesReceiver Receiver { get; set; }
        public DocumentOrderDocumentPartiesCreator Creator { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderDocumentPartiesSender
    {
        public string ILN { get; set; }
        public string CodeBySender { get; set; }
        public string CodeByReceiver { get; set; }
        public string Name { get; set; }
        public string TaxID { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderDocumentPartiesReceiver
    {
        public string ILN { get; set; }
        public string CodeBySender { get; set; }
        public string CodeByReceiver { get; set; }
        public string Name { get; set; }
        public string TaxID { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderDocumentPartiesCreator
    {
        public string SystemUniqueCode { get; set; }
        public string CodeBySender { get; set; }
        public string CodeByReceiver { get; set; }
        public string Name { get; set; }
        public string TelephoneNumber { get; set; }
        [XmlElement("E-mail")]
        public string Email { get; set; }
        public string CreationType { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderDetailsOfTransport
    {
        [XmlElement("Line-Transport")]
        public DocumentOrderDetailsOfTransportLineTransport LineTransport { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderDetailsOfTransportLineTransport
    {
        public object DespatchDateTime { get; set; }
        public object EndOfDespatchingTime { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public object EndOfLoadingTime { get; set; }
        public string TruckQuantity { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderParties
    {
        public DocumentOrderOrderPartiesBuyer Buyer { get; set; }
        public DocumentOrderOrderPartiesSeller Seller { get; set; }
        public DocumentOrderOrderPartiesDeliveryPoint DeliveryPoint { get; set; }
        public DocumentOrderOrderPartiesShipFrom ShipFrom { get; set; }
        public DocumentOrderOrderPartiesPayer Payer { get; set; }
        public DocumentOrderOrderPartiesInvoicee Invoicee { get; set; }
        public DocumentOrderOrderPartiesUltimateCustomer UltimateCustomer { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesBuyer
    {
        public string ILN { get; set; }
        public string CodeBySender { get; set; }
        public string CodeByReceiver { get; set; }
        public string Name { get; set; }
        public string CodeBySupplier { get; set; }
        public string CodeByBuyer { get; set; }
        public string TaxID { get; set; }
        public string AccountNumber { get; set; }
        public string UtilizationRegisterNumber { get; set; }
        public object RoomNumber { get; set; }
        public object Housing { get; set; }
        public object HouseNumber { get; set; }
        public string StreetAndNumber { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DocumentOrderOrderPartiesBuyerOperatorDetails OperatorDetails { get; set; }
        public DocumentOrderOrderPartiesBuyerLocalizationSettings LocalizationSettings { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesBuyerOperatorDetails
    {
        public string Name { get; set; }
        public string CodeByBuyer { get; set; }
        public string TelephoneNumber { get; set; }
        [XmlElement("E-mail")]
        public string Email { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesBuyerLocalizationSettings
    {
        public string PaymentTerms { get; set; }
        public string DiscountGroup { get; set; }
        public string AssortmentGroup { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesSeller
    {
        public string ILN { get; set; }
        public string CodeBySender { get; set; }
        public string CodeByReceiver { get; set; }
        public string Name { get; set; }
        public string CodeBySupplier { get; set; }
        public string CodeByBuyer { get; set; }
        public string TaxID { get; set; }
        public string AccountNumber { get; set; }
        public string UtilizationRegisterNumber { get; set; }
        public object RoomNumber { get; set; }
        public object Housing { get; set; }
        public object HouseNumber { get; set; }
        public string StreetAndNumber { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DocumentOrderOrderPartiesSellerSalesRepresentative SalesRepresentative { get; set; }
        public DocumentOrderOrderPartiesSellerOperatorDetails OperatorDetails { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesSellerSalesRepresentative
    {
        public string SystemUniqueCode { get; set; }
        public string Name { get; set; }
        public string TelephoneNumber { get; set; }
        [XmlElement("E-mail")]
        public string Email { get; set; }
        public string CodeBySeller { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesSellerOperatorDetails
    {
        public string Name { get; set; }
        public string CodeByBuyer { get; set; }
        public string TelephoneNumber { get; set; }
        [XmlElement("E-mail")]
        public string Email { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesDeliveryPoint
    {
        public string ILN { get; set; }
        public string SystemID { get; set; }
        public string CodeBySender { get; set; }
        public string CodeByReceiver { get; set; }
        public string Name { get; set; }
        public string CodeBySupplier { get; set; }
        public string CodeByBuyer { get; set; }
        public string TaxID { get; set; }
        public string AccountNumber { get; set; }
        public string UtilizationRegisterNumber { get; set; }
        public object RoomNumber { get; set; }
        public object Housing { get; set; }
        public object HouseNumber { get; set; }
        public string StreetAndNumber { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public object TradeFormat { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        [XmlElement("E-mail")]
        public string Email { get; set; }
        public string Web { get; set; }
        public DocumentOrderOrderPartiesDeliveryPointLocalizationSettings LocalizationSettings { get; set; }
        [XmlIgnore]
        public string FullAddress => $"{Country}, {PostalCode}, {State}, {CityName}, {StreetAndNumber}";
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesDeliveryPointLocalizationSettings
    {
        public string PaymentTerms { get; set; }
        public string DiscountGroup { get; set; }
        public string AssortmentGroup { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesShipFrom
    {
        public string ILN { get; set; }
        public string CodeBySender { get; set; }
        public string CodeByReceiver { get; set; }
        public string Name { get; set; }
        public string CodeBySupplier { get; set; }
        public string CodeByBuyer { get; set; }
        public string TaxID { get; set; }
        public string AccountNumber { get; set; }
        public string UtilizationRegisterNumber { get; set; }
        public object RoomNumber { get; set; }
        public object Housing { get; set; }
        public object HouseNumber { get; set; }
        public string StreetAndNumber { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        [XmlElement("E-mail")]
        public string Email { get; set; }
        public string Web { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesPayer
    {
        public string ILN { get; set; }
        public string CodeBySender { get; set; }
        public string CodeByReceiver { get; set; }
        public string Name { get; set; }
        public string CodeBySupplier { get; set; }
        public string CodeByBuyer { get; set; }
        public string TaxID { get; set; }
        public string AccountNumber { get; set; }
        public string UtilizationRegisterNumber { get; set; }
        public object RoomNumber { get; set; }
        public object Housing { get; set; }
        public object HouseNumber { get; set; }
        public string StreetAndNumber { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        [XmlElement("E-mail")]
        public string Email { get; set; }
        public string Web { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesInvoicee
    {
        public string ILN { get; set; }
        public string CodeBySender { get; set; }
        public string CodeByReceiver { get; set; }
        public string Name { get; set; }
        public string CodeBySupplier { get; set; }
        public string CodeByBuyer { get; set; }
        public string TaxID { get; set; }
        public string AccountNumber { get; set; }
        public string UtilizationRegisterNumber { get; set; }
        public object RoomNumber { get; set; }
        public object Housing { get; set; }
        public object HouseNumber { get; set; }
        public string StreetAndNumber { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        [XmlElement("E-mail")]
        public string Email { get; set; }
        public string Web { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderPartiesUltimateCustomer
    {
        public string ILN { get; set; }
        public string CodeBySender { get; set; }
        public string CodeByReceiver { get; set; }
        public string Name { get; set; }
        public string CodeBySupplier { get; set; }
        public string CodeByBuyer { get; set; }
        public string TaxID { get; set; }
        public string AccountNumber { get; set; }
        public string UtilizationRegisterNumber { get; set; }
        public object RoomNumber { get; set; }
        public object Housing { get; set; }
        public object HouseNumber { get; set; }
        public string StreetAndNumber { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        [XmlElement("E-mail")]
        public string Email { get; set; }
        public string Web { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLines
    {
        [XmlElement("Line")]
        public List<DocumentOrderOrderLinesLine> Lines { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLine
    {
        [XmlElement("Line-Item")]
        public DocumentOrderOrderLinesLineLineItem LineItem { get; set; }
        [XmlElement("Line-Delivery")]
        public DocumentOrderOrderLinesLineLineDelivery LineDelivery { get; set; }
        [XmlElement("Line-Control")]
        public DocumentOrderOrderLinesLineLineControl LineControl { get; set; }
        [XmlElement("Package-Identification")]
        public DocumentOrderOrderLinesLinePackageIdentification PackageIdentification { get; set; }
        [XmlElement("Line-Reference")]
        public DocumentOrderOrderLinesLineLineReference LineReference { get; set; }
        [XmlElement("Line-AdditionalData")]
        public DocumentOrderOrderLinesLineLineAdditionalData LineAdditionalData { get; set; }
        [XmlElement("Line-Contract")]
        public DocumentOrderOrderLinesLineLineContract LineContract { get; set; }
        [XmlElement("Package-Measurements")]
        public DocumentOrderOrderLinesLinePackageMeasurements PackageMeasurements { get; set; }

        [XmlIgnore]
        private bool isFailed;
        [XmlIgnore]
        public bool IsFailed
        {
            get { return isFailed; }
            set
            {
                isFailed = value;
                NotifyPropertyChanged("IsFailed");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineItem
    {
        public string LineNumber { get; set; }
        public string LineType { get; set; }
        public string SystemID { get; set; }
        public string BuyerLineNumber { get; set; }
        public string EAN { get; set; }
        public string BuyerItemCode { get; set; }
        public string SupplierItemCode { get; set; }
        public object GS1DataBar { get; set; }
        public string ItemDescription { get; set; }
        public string SupplierItemDescription { get; set; }
        public string ItemType { get; set; }
        public string OrderedQuantity { get; set; }
        public string RecommendedQuantity { get; set; }
        public string OrderedUnitPacksize { get; set; }
        public string BuyerUnitPacksize { get; set; }
        public string SupplierUnitPacksize { get; set; }
        public string UnitWeight { get; set; }
        public string ItemNumerator { get; set; }
        public string ItemDenumerator { get; set; }
        public string OrderedBoxes { get; set; }
        public string OrderedPallets { get; set; }
        public string UnitOfMeasure { get; set; }
        public string SupplierUnitOfMeasure { get; set; }
        public string SupplierStackingCode { get; set; }
        public string OrderedUnitNetPrice { get; set; }
        public string SupplierUnitNetPrice { get; set; }
        public string OrderedUnitGrossPrice { get; set; }
        public string OrderedNetAmount { get; set; }
        public string OrderedTaxAmount { get; set; }
        public string OrderedGrossAmount { get; set; }
        public string TaxRate { get; set; }
        public string Discount { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime ExpirationDate { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime MaxExpirationDate { get; set; }
        public string ItemStatusCode { get; set; }
        public string Remarks { get; set; }
        public string NetWeight { get; set; }
        public string GrossWeight { get; set; }
        public string Volume { get; set; }
        public string MinOrderedQuantity { get; set; }
        public string ProdcatVolume { get; set; }
        public string ProdcatPackSize { get; set; }
        public string ProdcatPalletSize { get; set; }
        public string ProdcatUnitNetWeight { get; set; }
        public string ProdcatWeight { get; set; }
        public string ExpirationPercent { get; set; }
        public string ProdcatBoxWeight { get; set; }
        public string ProdcatPalletWeight { get; set; }
        public object Caliber { get; set; }
        public object CountryOfOrigin { get; set; }
        public object ExpectedDespatchDate { get; set; }
        public object RequestedDeliveryDate { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineDelivery
    {
        [XmlElement(DataType = "date")]
        public DateTime ExpectedDeliveryDate { get; set; }
        public object LastExpectedDeliveryDate { get; set; }
        public string BuyerLocationCode { get; set; }
        public string SellerLocationCode { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineControl
    {
        [XmlElement("Line-UltimateCustomer")]
        public DocumentOrderOrderLinesLineLineControlLineUltimateCustomer LineUltimateCustomer { get; set; }
        public string EanCatalog { get; set; }
        public string SupplierItemCodeCatalog { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineControlLineUltimateCustomer
    {
        public string ILN { get; set; }
        public string CodeBySender { get; set; }
        public string CodeByReceiver { get; set; }
        public string Name { get; set; }
        public string CodeByBuyer { get; set; }
        public string CodeBySeller { get; set; }
        public string TaxID { get; set; }
        public string AccountNumber { get; set; }
        public string UtilizationRegisterNumber { get; set; }
        public object RoomNumber { get; set; }
        public object Housing { get; set; }
        public object HouseNumber { get; set; }
        public string StreetAndNumber { get; set; }
        public string CityName { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        [XmlElement("E-mail")]
        public string Email { get; set; }
        public string Web { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLinePackageIdentification
    {
        [XmlElement("Goods-Identity")]
        public DocumentOrderOrderLinesLinePackageIdentificationGoodsIdentity GoodsIdentity { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLinePackageIdentificationGoodsIdentity
    {
        public string Type { get; set; }
        public DocumentOrderOrderLinesLinePackageIdentificationGoodsIdentityRange Range { get; set; }
        public string QuantityPerPack { get; set; }
        public string BaseUnit { get; set; }
        public string PackagingUnit { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLinePackageIdentificationGoodsIdentityRange
    {
        [XmlElement("ID-Begin")]
        public string IDBegin { get; set; }
        [XmlElement("ID-End")]
        public string IDEnd { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineReference
    {
        [XmlElement("Reference-Elements")]
        public DocumentOrderOrderLinesLineLineReferenceReferenceElements ReferenceElements { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineReferenceReferenceElements
    {
        [XmlElement("Reference-Element")]
        public DocumentOrderOrderLinesLineLineReferenceReferenceElementsReferenceElement ReferenceElement { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineReferenceReferenceElementsReferenceElement
    {
        [XmlElement("Reference-Type")]
        public string ReferenceType { get; set; }
        [XmlElement("Reference-Id")]
        public string ReferenceId { get; set; }
        [XmlElement("Reference-Date", DataType = "date")]
        public DateTime ReferenceDate { get; set; }
        public DocumentOrderOrderLinesLineLineReferenceReferenceElementsReferenceElementAdditionalData AdditionalData { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineReferenceReferenceElementsReferenceElementAdditionalData
    {
        public string Content { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineAdditionalData
    {
        public string Content { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineContract
    {
        public string ContractNumber { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime ContractDate { get; set; }
        [XmlElement("Reference-Elements")]
        public DocumentOrderOrderLinesLineLineContractReferenceElements ReferenceElements { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineContractReferenceElements
    {
        [XmlElement("Reference-Element")]
        public DocumentOrderOrderLinesLineLineContractReferenceElementsReferenceElement ReferenceElement { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineContractReferenceElementsReferenceElement
    {
        [XmlElement("Reference-Type")]
        public string ReferenceType { get; set; }
        [XmlElement("Reference-Id")]
        public string ReferenceId { get; set; }
        [XmlElement("Reference-Date", DataType = "date")]
        public DateTime ReferenceDate { get; set; }
        public DocumentOrderOrderLinesLineLineContractReferenceElementsReferenceElementAdditionalData AdditionalData { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLineLineContractReferenceElementsReferenceElementAdditionalData
    {
        public string Content { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderLinesLinePackageMeasurements
    {
        public object GTIN { get; set; }
        public object StrictAcceptance { get; set; }
        public object MaxPalletWeight { get; set; }
        public object MaxPalletHeight { get; set; }
        public object PalletLayers { get; set; }
        public object UnitsPerBlock { get; set; }
        public object UnitsPerBox { get; set; }
        public object UnitsPerLayer { get; set; }
        public object UnitsPerPallet { get; set; }
        public object TypePacking { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderOrderSummary
    {
        public string TotalLines { get; set; }
        public string TotalOrderedAmount { get; set; }
        public string TotalNetAmount { get; set; }
        public string TotalGrossAmount { get; set; }
        public string TotalTaxAmount { get; set; }
        public string TotalNetWeight { get; set; }
        public string TotalGrossWeight { get; set; }
        public string TotalPalletAmount { get; set; }
        public string TotalVolume { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderDocumentAttachments
    {
        [XmlElement("Document-File")]
        public DocumentOrderDocumentAttachmentsDocumentFile DocumentFile { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderDocumentAttachmentsDocumentFile
    {
        [XmlElement("Attachment-Type")]
        public string AttachmentType { get; set; }
        [XmlElement("Content-Type")]
        public string ContentType { get; set; }
        [XmlElement("Content-Encoding")]
        public string ContentEncoding { get; set; }
        [XmlElement("File-Name")]
        public string FileName { get; set; }
        public string Content { get; set; }

    }
}

