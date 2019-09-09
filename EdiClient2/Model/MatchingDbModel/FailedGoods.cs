using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdiClient.Model.MatchingDbModel
{
    public class FailedGoods
    {

        public string ID_EDI_DOC { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public string EAN { get; set; }
        public string BUYER_ITEM_CODE { get; set; }
        public string ORDER_NUMBER { get; set; }
        public string ORDER_DATE { get; set; }
        public string SENDER_ILN { get; set; }

    }
}
