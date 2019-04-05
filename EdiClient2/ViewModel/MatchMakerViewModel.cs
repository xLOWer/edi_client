using EdiClient.Model.MatchingDbModel;
using EdiClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EdiClient.ViewModel
{
    public class MatchMakerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( info ) );
        }

        private Goods selectedGood { get; set; }
        public Goods SelectedGood
        {
            get { return selectedGood; }
            set
            {
                selectedGood = value;
                NotifyPropertyChanged( "SelectedGood" );
            }
        }

        private FailedGoods selectedFailedGood { get; set; }
        public FailedGoods SelectedFailedGood
        {
            get { return selectedFailedGood; }
            set
            {
                selectedFailedGood = value;
                NotifyPropertyChanged( "SelectedFailedGood" );
            }
        }

        private Matches selectedMatch { get; set; }
        public Matches SelectedMatch
        {
            get { return selectedMatch; }
            set
            {
                selectedMatch = value;
                NotifyPropertyChanged( "SelectedMatch" );
            }
        }

        private string goodSearchText { get; set; }
        public string GoodSearchText
        {
            get { return goodSearchText; }
            set
            {
                goodSearchText = value;
                NotifyPropertyChanged( "GoodSearchText" );
            }
        }

        private string failedGoodSearchText { get; set; }
        public string FailedGoodSearchText
        {
            get { return failedGoodSearchText; }
            set
            {
                failedGoodSearchText = value;
                NotifyPropertyChanged( "FailedGoodSearchText" );
            }
        }

        private List<Goods> goodsList { get; set; }
        public List<Goods> GoodsList
        {
            get { return goodsList; }
            set
            {
                goodsList = value;
                NotifyPropertyChanged( "GoodsList" );
            }
        }

        private List<FailedGoods> failedGoodsList { get; set; }
        public List<FailedGoods> FailedGoodsList
        {
            get { return failedGoodsList; }
            set
            {
                failedGoodsList = value;
                NotifyPropertyChanged( "FailedGoodsList" );
            }
        }

        private List<Matches> matchesList { get; set; }
        public List<Matches> MatchesList
        {
            get { return matchesList; }
            set
            {
                matchesList = value;
                NotifyPropertyChanged( "MatchesList" );
            }
        }


        public CommandService FailedGoodSearchCommand => new CommandService( FailedGoodSearch );
        public CommandService GoodSearchCommand => new CommandService( GoodSearch );
        public CommandService LoadDataCommand => new CommandService( LoadData );
        public CommandService MakeMatchingCommand => new CommandService( MakeMatching );
        public CommandService DisposeMatchingCommand => new CommandService( DisposeMatching );


        public void LoadData(object obj = null)
        {
            GoodsList = GetGoods();
            FailedGoodsList = GetFailedGoods();
            MatchesList = GetMatchesList();
        }
        public void MakeMatching(object obj = null)
        {

        }
        public void DisposeMatching(object obj = null)
        {

        }
        public void FailedGoodSearch(object obj = null)
        {

        }
        public void GoodSearch(object obj = null)
        {

        }

        private List<Goods> GetGoods()
        {
            var list = new List<string>();
            list.Add( SqlConfiguratorService.Sql_SelectGoods( GoodSearchText ) );
            return DbService<Goods>.DocumentSelect( list );
        }

        private List<FailedGoods> GetFailedGoods()
        {
            var list = new List<string>();
            list.Add( SqlConfiguratorService.Sql_SelectFailedGoods( FailedGoodSearchText ) );
            return DbService<FailedGoods>.DocumentSelect( list );
        }

        private List<Matches> GetMatchesList()
        {
            var list = new List<string>();
            list.Add( SqlConfiguratorService.Sql_SelectMatches( ) );
            return DbService<Matches>.DocumentSelect( list );
        }

        public MatchMakerViewModel()
        {

        }

    }
}
