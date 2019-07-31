using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EdiClient.Model
{
    public class Document
    {
        public string ID { get; set; }
        public string ID_CUSTOMER { get; set; }
        public string ID_TRADER { get; set; }
        public string ORDER_NUMBER { get; set; }
        public string ORDER_DATE { get; set; }
        public string ORDER { get; set; }
        public string ORDRSP { get; set; }
        public string DESADV { get; set; }
        public string FAILED { get; set; }
        public string TOTAL_GROSS_AMOUNT { get; set; }
        public string SENDER_ILN { get; set; }
        public string DELIVERY_POINT_ILN { get; set; }
        public string BUYER_ILN { get; set; }
        public string CUSTOMER_ILN { get; set; }
        public string SELLER_ILN { get; set; }
        public string ORDER_CURRENCY { get; set; }
        public string EXPECTED_DELIVERY_DATE { get; set; }
        public string REMARKS { get; set; }
        public string CREATOR { get; set; }
        public string CODE { get; set; }
        public string DOC_DATETIME { get; set; }
        public string ACT_STATUS { get; set; }
        public string CONTRACTOR_MANE { get; set; }
        
        public string DocumentType
        {
            get
            {
                if (IsDesadv) return "6 Отгрузка";
                if (IsReadyToDesadv) return "5 Готово к отгрузке";
                if (IsOrdrsp) return "4 Ответ";
                if (IsReadyToOrdrsp) return "3 Готово к ответу";
                if (IsInTrader && !IsOrdrsp && !IsDesadv) return "2 В трейдере";
                if (IsReadyToTrader) return "1 Заказ";
                if (IsFailed) return "0 Ошибка: нет связи товара";
                if(IsEmptyContractorLink) return "0 Ошибка: нет связи точки";

                return "0 Ошибка: прочее";
            }
        }

        public bool IsFailed => FAILED == "1";
        public bool IsReadyToTrader => !IsInTrader && !IsOrdrsp && !IsDesadv && !IsEmptyContractorLink;
        public bool IsReadyToOrdrsp => IsInTrader && !IsDesadv && int.Parse(ACT_STATUS) == 3;
        public bool IsReadyToDesadv => IsOrdrsp && int.Parse(ACT_STATUS) == 4;

        public bool IsEmptyContractorLink => string.IsNullOrEmpty(CONTRACTOR_MANE) && !IsFailed;

        public bool IsInTrader => !string.IsNullOrEmpty(ID_TRADER) && !IsFailed;
        public bool IsOrdrsp => !string.IsNullOrEmpty(ORDRSP) && !IsFailed;
        public bool IsDesadv => !string.IsNullOrEmpty(DESADV) && !IsFailed;
        //public bool IsRecadv => !string.IsNullOrEmpty(RECADV) ? true : false;

        public int Details_DeliveryLinesCount => Details.Where(x => x.QUANTITY != "0" || string.IsNullOrEmpty(x.QUANTITY)).Count();
        public long Details_OrderedLinesCount => Details.Sum(x => long.Parse(x.ORDERED_QUANTITY));
        public long Details_DeliveryPositionCount => Details.Sum(x => long.Parse(x.QUANTITY));
        public double Details_DeliveryGrossSumm => Details.Sum(x => double.Parse(x.PRICE) * int.Parse(x.QUANTITY));
        
        public int DetailsCount => Details?.Count ?? 0;
        public List<Detail> Details { get; set; }
    }
}