using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdiClient.Model.MatchingDbModel
{
    public class Client
    {
        public string SENDER_GLN { get; set; }
        public string BUYER_GLN { get; set; }
        public string DELIVERY_POINT_GLN { get; set; }

        public string DELIVERY_POINT_NAME { get; set; }
        public string BUYER_NAME { get; set; }

        public string SENDER_CUSTOMER_ID { get; set; }
        public string BUYER_CUSTOMER_ID { get; set; }
        public string DELIVERY_POINT_CONTRACTOR_ID { get; set; }

        public string SENDER_CUSTOMER { get; set; }
        public string BUYER_CUSTOMER { get; set; }
        public string DELIVERY_POINT_CONTRACTOR { get; set; }   
    }
}



    