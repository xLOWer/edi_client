using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace EdiClient.Model.WebModel
{
    [System.Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("mailbox-response", Namespace = "", IsNullable = false)]
    public partial class MailboxResponse
    {
        [XmlElement("document-info")]
        public List<DocumentInfo> DocumentInfo { get; set; }
    }

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("document-info", Namespace = "", IsNullable = false)]
    public partial class DocumentInfo
    {        
        [XmlElement("partner-iln")]                     public string partneriln { get; set; }        
        [XmlElement("tracking-id")]                     public string trackingId { get; set; }        
        [XmlElement("document-type")]                   public string documentType { get; set; }        
        [XmlElement("document-version")]                public string documentVersion { get; set; }        
        [XmlElement("document-standard")]               public string documentStandard { get; set; }        
        [XmlElement("document-test")]                   public string documentTest { get; set; }        
        [XmlElement("document-status")]                 public string documentStatus { get; set; }        
        [XmlElement("document-number")]                 public string documentNumber { get; set; }        
        [XmlElement("document-date")]                   public string documentDate { get; set; }        
        [XmlElement("document-control-number")]         public string documentControlNumber { get; set; }        
        [XmlElement("receive-date")]                    public string receiveDate { get; set; }        
        [XmlElement("file-name")]                       public string fileName { get; set; }
    }


    
}
