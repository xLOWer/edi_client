using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace EdiClient.Model
{
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRootAttribute("Document-DespatchAdvice", Namespace = "", IsNullable = false)]
    public partial class DocumentDespatchAdvice
    {
        [XmlElement("DespatchAdvice-Header")]
        public DocumentDespatchAdviceDespatchAdviceHeader DespatchAdviceHeader { get; set; }
        [XmlElement("Document-Parties")]
        public DocumentDespatchAdviceDocumentParties DocumentParties { get; set; }
        [XmlElement("DespatchAdvice-Transport")]
        public DocumentDespatchAdviceDespatchAdviceTransport DespatchAdviceTransport { get; set; }
        [XmlElement("DespatchAdvice-Parties")]
        public DocumentDespatchAdviceDespatchAdviceParties DespatchAdviceParties { get; set; }
        [XmlElement("DespatchAdvice-Consignment")]
        public DocumentDespatchAdviceDespatchAdviceConsignment DespatchAdviceConsignment { get; set; }
        [XmlElement("DespatchAdvice-Summary")]
        public DocumentDespatchAdviceDespatchAdviceSummary DespatchAdviceSummary { get; set; }

        [XmlIgnore]
        public bool IsInEdiAsDesadv { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceHeader
    {
        public string DespatchAdviceNumber { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime DespatchAdviceDate { get; set; }
        public string BillOfLadingNumber { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime BillOfLadingDate { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime EstimatedDeliveryDate { get; set; }
        public string EstimatedDeliveryTime { get; set; }
        public string BuyerOrderNumber { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime BuyerOrderDate { get; set; }
        public string UTDnumber { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime UTDDate { get; set; }
        public string DocumentFunctionCode { get; set; }
        public string DocumentNameCode { get; set; }
        public string Remarks { get; set; }
        public DocumentDespatchAdviceDespatchAdviceHeaderReference Reference { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceHeaderReference
    {
        public string ContractNumber { get; set; }
        [XmlArray("Reference-Elements")]
        [XmlArrayItem("Reference-Element", IsNullable = false)]
        public List<DocumentDespatchAdviceDespatchAdviceHeaderReferenceReferenceElement> ReferenceElements { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceHeaderReferenceReferenceElement
    {
        [XmlElement("Reference-Type")]
        public string ReferenceType { get; set; }
        [XmlElement("Reference-Id")]
        public string ReferenceId { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDocumentParties
    {
        public DocumentDespatchAdviceDocumentPartiesSender Sender { get; set; }
        public DocumentDespatchAdviceDocumentPartiesReceiver Receiver { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDocumentPartiesSender
    {
        public string ILN { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDocumentPartiesReceiver
    {
        public string ILN { get; set; }
        public string Name { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceTransport
    {
        public string ConveyanceReferenceNumber { get; set; }
        public string ModeOfTransport { get; set; }
        public string VehicleType { get; set; }
        public string CarrierName { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime DispatchDateOfDelivery { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime ArrivalDateOfDelivery { get; set; }
        public string Capacity { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceParties
    {
        public DocumentDespatchAdviceDespatchAdvicePartiesBuyer Buyer { get; set; }
        public DocumentDespatchAdviceDespatchAdvicePartiesSeller Seller { get; set; }
        public DocumentDespatchAdviceDespatchAdvicePartiesDeliveryPoint DeliveryPoint { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdvicePartiesBuyer
    {
        public string ILN { get; set; }
        public string Name { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdvicePartiesSeller
    {
        public string ILN { get; set; }
        public string CodeByBuyer { get; set; }
        public string TaxID { get; set; }
        public string TaxRegistrationReasonCode { get; set; }
        public string Name { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdvicePartiesDeliveryPoint
    {
        public string ILN { get; set; }
        public string Name { get; set; }
        public string StreetAndNumber { get; set; }
        public string CityName { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceConsignment
    {
        [XmlArray("Packing-Sequence")]
        [XmlArrayItem("Line", IsNullable = false)]
        public List<DocumentDespatchAdviceDespatchAdviceConsignmentLine> PackingSequence { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceConsignmentLine
    {
        [XmlElement("Line-Item")]
        public DocumentDespatchAdviceDespatchAdviceConsignmentLineLineItem LineItem { get; set; }
        [XmlElement("Line-Order")]
        public DocumentDespatchAdviceDespatchAdviceConsignmentLineLineOrder LineOrder { get; set; }
        [XmlElement("Package-Identification")]
        public DocumentDespatchAdviceDespatchAdviceConsignmentLinePackageIdentification PackageIdentification { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceConsignmentLineLineItem
    {
        public string LineNumber { get; set; }
        public string EAN { get; set; }
        public string BuyerItemCode { get; set; }
        public string SupplierItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemType { get; set; }
        public string OrderedQuantity { get; set; }
        public string QuantityDespatched { get; set; }
        public string UnitPacksize { get; set; }
        public string UnitVolume { get; set; }
        public string UnitOfMeasure { get; set; }
        public string DespatchDate { get; set; }
        [XmlIgnore]
        public DateTime ExpirationDate { get; set; }
        [XmlIgnore]
        public DateTime ProductionDate { get; set; }
        public string BatchNumber { get; set; }
        public string ItemStatusCode { get; set; }
        public string ColourCode { get; set; }
        public string ItemSize { get; set; }
        public string SupplierPackCode { get; set; }
        public string UnitNetPrice { get; set; }
        public string UnitGrossPrice { get; set; }
        public string TaxRate { get; set; }
        public string SuggestedPrice { get; set; }
        public string MaxRetailPrice { get; set; }
        public string NetAmount { get; set; }
        public string TaxAmount { get; set; }
        public string GrossAmount { get; set; }
        public string Remarks { get; set; }
        public string ExpirationPercent { get; set; }
        public string Caliber { get; set; }
        public string PalletQuantity { get; set; }
        public string BoxesQuantity { get; set; }
        public string UnitNetWeight { get; set; }
        public string UnitGrossWeight { get; set; }
        public string TemperatureMode { get; set; }
        
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceConsignmentLineLineOrder
    {
        public string BuyerLineNumber { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceConsignmentLinePackageIdentification
    {
        [XmlElement("Goods-Identity")]
        public DocumentDespatchAdviceDespatchAdviceConsignmentLinePackageIdentificationGoodsIdentity GoodsIdentity { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceConsignmentLinePackageIdentificationGoodsIdentity
    {
        public string Type { get; set; }
        public DocumentDespatchAdviceDespatchAdviceConsignmentLinePackageIdentificationGoodsIdentityRange Range { get; set; }
        public string QuantityPerPack { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceConsignmentLinePackageIdentificationGoodsIdentityRange
    {
        [XmlElement("ID-Begin")]
        public string IDBegin { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentDespatchAdviceDespatchAdviceSummary
    {
        /// <summary>
        /// Количество упаковок
        /// </summary>
        public string TotalPSequence { get; set; }
        public string TotalLines { get; set; }
        /// <summary>
        /// Товаров
        /// </summary>
        public string TotalGoodsDespatchedAmount { get; set; }
        /// <summary>
        /// Сумма
        /// </summary>
        public string TotalNetAmount { get; set; }
        /// <summary>
        /// Сумма+НДС
        /// </summary>
        public string TotalGrossAmount { get; set; }
        /// <summary>
        /// НДС
        /// </summary>
        public string TotalTaxAmount { get; set; }
    }
}