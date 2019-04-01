using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace EdiClient.Model.WebModel
{
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("organization-info", Namespace = "", IsNullable = false)]
    public partial class OrganizationInfo
    {
        public string address { get; set; }
        public string boxId { get; set; }
        public string city { get; set; }
        public string fnsId { get; set; }
        public ulong gln { get; set; }
        public ulong inn { get; set; }
        public ulong kpp { get; set; }
        public string nameFull { get; set; }
        public string nameShort { get; set; }
        public ulong ogrn { get; set; }
        public string region { get; set; }
        public byte stateCode { get; set; }
    }
}
