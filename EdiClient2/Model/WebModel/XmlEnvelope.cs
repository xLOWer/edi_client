using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdiClient.Model.WebModel
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
    public partial class Envelope
    {
        public EnvelopeBody Body { get; set; }
    }


    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBody
    {
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:wsedi")]
        public receiveResponse receiveResponse { get; set; }
    }


    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:wsedi")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:wsedi", IsNullable = false)]
    public partial class receiveResponse
    {
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
        public @return @return { get; set; }
    }


    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class @return
    {
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:wsedi")]
        public object cnt { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:wsedi")]
        public byte res { get; set; }
    }
}
