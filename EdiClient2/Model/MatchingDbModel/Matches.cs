using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdiClient.Model.MatchingDbModel
{
    public class Matches
    {
        public string CustomerGln { get; set; }
        public string CustomerGoodId { get; set; }
        public string GoodId { get; set; }
        public string BarCode { get; set; }
        public string Name { get; set; }
    }
}

/*
SELECT rgm.CUSTOMER_GLN "CustomerGln"
  ,rgm.CUSTOMER_ARTICLE "CustomerGoodId"
  ,rgm.ID_GOOD "GoodId"
  ,(SELECT NAME FROM ref_goods WHERE ID = rgm.ID_GOOD) "Name"
  FROM REF_GOODS_MATCHING rgm
  WHERE DISABLED = 0;
*/
