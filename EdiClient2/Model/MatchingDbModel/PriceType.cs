namespace EdiClient.Model.MatchingDbModel
{
    public class PriceType
    {
        public string PriceId { get; set; }
        public string PriceName { get; set; }
        public string PriceCoef { get; set; }
        public string CurrencyName { get; set; }
        public string ParentName { get; set; }
    }
}
/*
PriceId	    PriceName	                PriceCoef	CurrencyName	ParentName
120501	    Дистр. Владивосток /РЕМИ	1.05	    Рубль	        Базовая
*/
