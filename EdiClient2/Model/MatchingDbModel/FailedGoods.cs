using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdiClient.Model.MatchingDbModel
{
    public class FailedGoods
    {
        public string GoodId { get; set; }
        public string EdiDocId { get; set; }
        public string LineNumber { get; set; }
        public string ItemDescription { get; set; }
        public string Ean { get; set; }
        public string BuyerItemCode { get; set; }
        public string OrderName { get; set; }
    }
}

/*
  SELECT id_good "GoodId"
    , EDD.ID_EDI_DOC "EdiDocId"
    , EDD.LINE_NUMBER "LineNumber"
    ,EDD.ITEM_DESCRIPTION "ItemDescription"
    ,EDD.EAN "Ean"
    ,EDD.BUYER_ITEM_CODE "BuyerItemCode"
    ,(SELECT ORDER_NUMBER||' от '||ORDER_DATE FROM HPCSERVICE.EDI_DOC WHERE id = edd.ID_EDI_DOC) "OrderName"
    FROM HPCSERVICE.EDI_DOC_DETAILS EDD
    WHERE EDD.FAILED = 1
*/
