namespace EdiClient.Model.MatchingDbModel
{
    public class Goods
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string BarCode { get; set; }
        public string GoodSize { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerId { get; set; }
        public string ExpiringDate { get; set; }
        public string RegNum { get; set; }
        public string SertNum { get; set; }
        public string SubdivisionId { get; set; }
    }
}
/*
  SELECT rg.id "Id"
    , rg.code "Code"
    , rg.name "Name"
    , rg.bar_code "BarCode"
    , rg.GOOD_SIZE "GoodSize"
    , (SELECT name FROM REF_CONTRACTORS WHERE ID = rg.id_manufacturer) "Manufacturer"
    , rg.ID_MANUFACTURER "ManufacturerId"
    , rg.EXPIRING_DATE "ExpiringDate"
    , rg.REG_NUM "RegNum"
    , rg.SERT_NUM "SertNum"
    , rg.ID_SUBDIVISION "SubdivisionId"
  FROM ref_goods rg
  WHERE name like '%%';
*/
