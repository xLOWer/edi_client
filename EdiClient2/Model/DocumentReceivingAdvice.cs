using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace EdiClient.Model
{

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("Document-ReceivingAdvice", Namespace = "", IsNullable = false)]
    public partial class DocumentReceivingAdvice
    {
        [XmlElement("ReceivingAdvice-Header")]
        public DocumentReceivingAdviceReceivingAdviceHeader ReceivingAdviceHeader { get; set; }
        [XmlElement("Document-Parties")]
        public DocumentReceivingAdviceDocumentParties DocumentParties { get; set; }
        [XmlElement("ReceivingAdvice-Parties")]
        public DocumentReceivingAdviceReceivingAdviceParties ReceivingAdviceParties { get; set; }
        [XmlArray("ReceivingAdvice-Lines")]
        [XmlArrayItem("Line", IsNullable = false)]
        public List<DocumentReceivingAdviceLine> ReceivingAdviceLines { get; set; }
        [XmlElement("ReceivingAdvice-Summary")]
        public DocumentReceivingAdviceReceivingAdviceSummary ReceivingAdviceSummary { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceReceivingAdviceHeader
    {
        public string ReceivingAdviceNumber { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime ReceivingAdviceDate { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime GoodsReceiptDate { get; set; }
        public string BuyerOrderNumber { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime BuyerOrderDate { get; set; }
        public string DespatchNumber { get; set; }
        public string DocumentFunctionCode { get; set; }
        public string DocumentNameCode { get; set; }
        public DocumentReceivingAdviceReceivingAdviceHeaderReference Reference { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceReceivingAdviceHeaderReference
    {
        [XmlArray("Reference-Elements")]
        [XmlArrayItem("Reference-Element", IsNullable = false)]
        public List<DocumentReceivingAdviceReceivingAdviceHeaderReferenceReferenceElement> ReferenceElements { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceReceivingAdviceHeaderReferenceReferenceElement
    {
        [XmlElement("Reference-Type")]
        public string ReferenceType { get; set; }
        [XmlElement("Reference-Id")]
        public string ReferenceId { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceDocumentParties
    {
        public DocumentReceivingAdviceDocumentPartiesSender Sender { get; set; }
        public DocumentReceivingAdviceDocumentPartiesReceiver Receiver { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceDocumentPartiesSender
    {
        public string ILN { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceDocumentPartiesReceiver
    {
        public string ILN { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceReceivingAdviceParties
    {
        public DocumentReceivingAdviceReceivingAdvicePartiesBuyer Buyer { get; set; }
        public DocumentReceivingAdviceReceivingAdvicePartiesSeller Seller { get; set; }
        public DocumentReceivingAdviceReceivingAdvicePartiesDeliveryPoint DeliveryPoint { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceReceivingAdvicePartiesBuyer
    {
        public string ILN { get; set; }
        public string Name { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceReceivingAdvicePartiesSeller
    {
        public string ILN { get; set; }
        public string Name { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceReceivingAdvicePartiesDeliveryPoint
    {
        public string ILN { get; set; }
        public string Name { get; set; }
        public string StreetAndNumber { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceLine
    {
        [XmlElement("Line-Item")]
        public DocumentReceivingAdviceLineLineItem LineItem { get; set; }
        [XmlElement("Line-Reference")]
        public DocumentReceivingAdviceLineLineReference LineReference { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceLineLineItem
    {
        public string LineNumber { get; set; }
        public string EAN { get; set; }
        public string BuyerItemCode { get; set; }
        public string QuantityOrdered { get; set; }
        public string QuantityReceived { get; set; }
        public string UnitPacksize { get; set; }
        public string UnitGrossPrice { get; set; }
        public string UnitNetPrice { get; set; }
        public string UnitOfMeasure { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceLineLineReference
    {
        public string RefLineNumber { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentReceivingAdviceReceivingAdviceSummary
    {
        public string TotalLines { get; set; }
        public string TotalGoodsReceiptAmount { get; set; }
        public string TotalNetAmount { get; set; }
        public string TotalGrossAmount { get; set; }
    }

}
