using System;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace EdiClient.Model.WebModel
{
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("relation-response", Namespace = "", IsNullable = false)]
    public partial class RelationResponse
    {
        [XmlElement("relation")]
        public List<Relation> Relations { get; set; }

        [Serializable()]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true)]
        public partial class Relation
        {
            [XmlIgnore]
            public string Name => $"{partnerIln}, {partnerName}, {documentType}";

            [XmlElement("relation-id")]         public string relationId { get; set; }
            [XmlElement("partner-iln")]         public string partnerIln { get; set; }
            [XmlElement("partner-name")]        public string partnerName { get; set; }
            [XmlElement("document-type")]       public string documentType { get; set; }
            [XmlElement("document-version")]    public string documentVersion { get; set; }
            [XmlElement("document-standard")]   public string documentStandard { get; set; }
            [XmlElement("document-test")]       public string documentTest { get; set; }

            public string direction { get; set; }
            public string description { get; set; }
            public string test { get; set; }
            public string form { get; set; }
        }
    }
}