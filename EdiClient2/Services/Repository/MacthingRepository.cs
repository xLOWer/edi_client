using EdiClient.Model.MatchingDbModel;
using System.Collections.Generic;

namespace EdiClient.Services.Repository
{
    public static class MacthingRepository
    {
        public static List<Goods> GoodsList { get;set;}
        public static List<FailedGoods> FailedGoodsList { get;set;}
        public static List<Matches> MatchesList { get;set;}

        internal static void Get() { }
    }
}
