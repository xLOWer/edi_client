using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace EdiClient.Model
{
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRootAttribute("Document-OrderResponse", Namespace = "", IsNullable = false)]
    public partial class DocumentOrderResponse
    {
        [XmlElement("OrderResponse-Header")]
        public DocumentOrderResponseOrderResponseHeader OrderResponseHeader { get; set; }
        [XmlElement("Document-Parties")]
        public DocumentOrderResponseDocumentParties DocumentParties { get; set; }
        public DocumentOrderResponseDetailsOfTransport DetailsOfTransport { get; set; }
        [XmlElement("OrderResponse-Parties")]
        public DocumentOrderResponseOrderResponseParties OrderResponseParties { get; set; }
        [XmlElement("OrderResponse-Lines")]        
        public DocumentOrderResponseLine OrderResponseLines { get; set; }
        [XmlElement("OrderResponse-Summary")]
        public DocumentOrderResponseOrderResponseSummary OrderResponseSummary { get; set; }

        [XmlIgnore]
        public bool IsInEdiAsOrdrsp { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseOrderResponseHeader
    {
        public string OrderResponseNumber { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime OrderResponseDate { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime ExpectedDeliveryDate { get; set; }
        public string OrderResponseCurrency { get; set; }
        public string DocumentFunctionCode { get; set; }
        public DocumentOrderResponseOrderResponseHeaderOrder Order { get; set; }
        public string ReasonCode { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseOrderResponseHeaderOrder
    {
        public string BuyerOrderNumber { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime BuyerOrderDate { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseDocumentParties
    {
        public DocumentOrderResponseDocumentPartiesSender Sender { get; set; }
        public DocumentOrderResponseDocumentPartiesReceiver Receiver { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseDocumentPartiesSender
    {
        public string ILN { get; set; }
        public string Name { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseDocumentPartiesReceiver
    {
        public string ILN { get; set; }
        public string Name { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseDetailsOfTransport
    {
        public string TemperatureMode { get; set; }
        [XmlElement("Line-Transport")]
        public DocumentOrderResponseDetailsOfTransportLineTransport LineTransport { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseDetailsOfTransportLineTransport
    {
        public DateTime DeliveryDateTime { get; set; }
        public DateTime ShipmentStartDateTime { get; set; }
        public DateTime ShipmentEndDateTime { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseOrderResponseParties
    {
        public DocumentOrderResponseOrderResponsePartiesBuyer Buyer { get; set; }
        public DocumentOrderResponseOrderResponsePartiesSeller Seller { get; set; }
        public DocumentOrderResponseOrderResponsePartiesDeliveryPoint DeliveryPoint { get; set; }
        public DocumentOrderResponseOrderResponsePartiesShipFrom ShipFrom { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseOrderResponsePartiesBuyer
    {
        public string ILN { get; set; }
        public string Name { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseOrderResponsePartiesSeller
    {
        public string ILN { get; set; }
        public string Name { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseOrderResponsePartiesDeliveryPoint
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
    public partial class DocumentOrderResponseOrderResponsePartiesShipFrom
    {
        public string ILN { get; set; }
        public string Name { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseLine
    {
        [XmlElement("Line")]
        public List<Line> Lines { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class Line
    {
        [XmlElement("Line-Item")]
        public DocumentOrderResponseLineLineItem LineItem { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseLineLineItem
    {
        public string LineNumber { get; set; }
        public string EAN { get; set; }
        public string BuyerItemCode { get; set; }
        public string SupplierItemCode { get; set; }
        public string GS { get; set; }
        public string ItemDescription { get; set; }
        public string SubstituteEAN { get; set; }
        public string BuyerSubstituteItemCode { get; set; }
        public string SupplierSubstituteItemCode { get; set; }
        public string SubstituteItemDescription { get; set; }
        public string ItemStatus { get; set; }
        public string ItemType { get; set; }
        public string OrderedQuantity { get; set; }
        public string RecommendedQuantity { get; set; }
        public string AllocatedDelivered { get; set; }
        public string QuantityToBeDelivered { get; set; }
        public string QuantityDifference { get; set; }
        public string OrderedUnitPacksize { get; set; }
        public string UnitOfMeasure { get; set; }
        public string OrderedUnitNetPrice { get; set; }
        public string OrderedUnitGrossPrice { get; set; }
        public string SuggestedPrice { get; set; }
        public string TaxRate { get; set; }
        public string Discount { get; set; }
        public string NetAmount { get; set; }
        public string TaxAmount { get; set; }
        public string GrossAmount { get; set; }
        public string ExpirationDate { get; set; }
        public string ExpectedDeliveryDate { get; set; }
        public string ExpectedDeliveryTime { get; set; }
        public string SpecialConditionCode { get; set; }
        public string CountryOfOrigin { get; set; }
        public string UnitWeight { get; set; }
        public string NetWeight { get; set; }
        public string GrossWeight { get; set; }
        public string BoxWeight { get; set; }
        public string PalletWeight { get; set; }
        public string Caliber { get; set; }
        public string PalletQuantity { get; set; }
        public string BoxesQuantity { get; set; }
    }
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class DocumentOrderResponseOrderResponseSummary
    {
        public string TotalLines { get; set; }
        public string TotalPackages { get; set; }
        public string TotalAmount { get; set; }
        public string TotalNetAmount { get; set; }
        public string TotalTaxAmount { get; set; }
        public string TotalGrossAmount { get; set; }
    }
}