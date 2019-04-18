namespace EdiClient.Model.MatchingDbModel
{
    public class MatchingPriceTypes
    {
        public string CustomerGln { get; set; }
        public string IdPriceType { get; set; }
        public string Disabled { get; set; }
        public string DisabledDatetime { get; set; }
        public string InsertDatetime { get; set; }
        public string InsertUser { get; set; }
        public string PriceTypeName { get; set; }
        public string CustomerName { get; set; }
    }
}
/*
  CUSTOMER_GLN      NUMBER(*, 0),
  ID_PRICE_TYPE           NUMBER(*, 0),
  DISABLED          NUMBER(1, 0),
  DISABLED_DATETIME DATE,
  INSERT_DATETIME   DATE              DEFAULT SYSDATE,
  INSERT_USER       VARCHAR2(10 BYTE) DEFAULT USER
*/
