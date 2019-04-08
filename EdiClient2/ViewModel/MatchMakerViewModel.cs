using Devart.Data.Oracle;
using EdiClient.Model.MatchingDbModel;
using EdiClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Controls;
using EdiClient.View;

namespace EdiClient.ViewModel
{
    public class MatchMakerViewModel : INotifyPropertyChanged
    {
        private MatchMakerView _page { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaiseNotifyPropertyChanged( string info)
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( info ) );
        }
        

        private bool isNewMatchingEnabled = true;
        public virtual bool IsNewMatchingEnabled
        {
            get
            {
                return !String.IsNullOrEmpty( NewCustomerItemCode );/* || (NewCustomerItemCode?.Length ?? 0) >= 4 ) && SelectedGood != null*/;
            }
            set
            {
                isNewMatchingEnabled = value;
                RaiseNotifyPropertyChanged( "IsNewMatchingEnabled" );
            }
        }


        private bool disposeEnabled = true;
        public bool DisposeEnabled
        {
            get
            {
                return !String.IsNullOrEmpty(SelectedMatch?.CustomerGoodId ?? ""); }
            set
            {
                disposeEnabled = value;
                RaiseNotifyPropertyChanged( "DisposeEnabled" );
            }
        }

        private bool linkEnabled = true;
        public bool LinkEnabled
        {
            get { return !String.IsNullOrEmpty( SelectedGood?.Id??"") && !String.IsNullOrEmpty( SelectedFailedGood?.BuyerItemCode??""); }
            set
            {
                linkEnabled = value;
                RaiseNotifyPropertyChanged( "LinkEnabled" );
            }
        }

        private Goods selectedGood = new Goods();
        public Goods SelectedGood
        {
            get { return selectedGood; }
            set
            {
                selectedGood = value;
                RaiseNotifyPropertyChanged( "SelectedGood" );
            }
        }

        private FailedGoods selectedFailedGood = new FailedGoods();
        public FailedGoods SelectedFailedGood
        {
            get { return selectedFailedGood; }
            set
            {
                selectedFailedGood = value;
                RaiseNotifyPropertyChanged( "SelectedFailedGood" );
            }
        }

        private Matches selectedMatch = new Matches();
        public Matches SelectedMatch
        {
            get { return selectedMatch; }
            set
            {
                selectedMatch = value;
                RaiseNotifyPropertyChanged( "SelectedMatch" );
            }
        }
                      

        private string newCustomerItemCode = "";
        public string NewCustomerItemCode
        {
            get { return newCustomerItemCode; }
            set
            {
                newCustomerItemCode = value;
                RaiseNotifyPropertyChanged( "NewCustomerItemCode" );
            }
        }

        private string goodSearchText = "";
        public string GoodSearchText
        {
            get { return goodSearchText; }
            set
            {
                goodSearchText = value;
                RaiseNotifyPropertyChanged( "GoodSearchText" );
            }
        }

        private string failedGoodSearchText = "";
        public string FailedGoodSearchText
        {
            get { return failedGoodSearchText; }
            set
            {
                failedGoodSearchText = value;
                RaiseNotifyPropertyChanged( "FailedGoodSearchText" );
            }
        }

        private string matchesSearchText = "";
        public string MatchesSearchText
        {
            get { return matchesSearchText; }
            set
            {
                matchesSearchText = value;
                RaiseNotifyPropertyChanged( "MatchesSearchText" );
            }
        }

        private List<Goods> fullGoodsList = new List<Goods>();

        private List<Goods> goodsList = new List<Goods>();
        public List<Goods> GoodsList
        {
            get { return goodsList; }
            set
            {
                goodsList = value;
                RaiseNotifyPropertyChanged( "GoodsList" );
            }
        }

        private List<FailedGoods> failedGoodsList = new List<FailedGoods>();
        public List<FailedGoods> FailedGoodsList
        {
            get { return failedGoodsList; }
            set
            {
                failedGoodsList = value;
                RaiseNotifyPropertyChanged( "FailedGoodsList" );
            }
        }

        private List<Matches> matchesList = new List<Matches>();
        public List<Matches> MatchesList
        {
            get { return matchesList; }
            set
            {
                matchesList = value;
                RaiseNotifyPropertyChanged( "MatchesList" );
            }
        }
        

        public CommandService NewMatchingCommand => new CommandService( NewMatching );
        public CommandService FailedGoodSearchCommand => new CommandService( FailedGoodSearch );
        public CommandService MatchesSearchCommand => new CommandService( MatchesSearch );
        public CommandService GoodSearchCommand => new CommandService( GoodSearch );
        public CommandService LoadDataCommand => new CommandService( LoadData );
        public CommandService MakeMatchingCommand => new CommandService( MakeMatching );
        public CommandService DisposeMatchingCommand => new CommandService( DisposeMatching );


        public void NewMatching(object obj = null)
        {
            if (NewCustomerItemCode.Length < 4 || NewCustomerItemCode == null)
            {
                MessageBox.Show("Поле с кодом не должно быть короче 4 символов и быть пустым");
                return;
            }
            DbService.Insert( $@"insert abt.REF_GOODS_MATCHING(CUSTOMER_GLN,CUSTOMER_ARTICLE,ID_GOOD,DISABLED)
values('4650093209994',{NewCustomerItemCode},{SelectedGood.Id},7)" );

            NewCustomerItemCode = "";

            FailedGoodsList = GetFailedGoods();
            MatchesList = GetMatchesList();
        }

        public void LoadData(object obj = null)
        {
            _page.LoadDataButton.IsEnabled = false;
            _page.UpdateLayout();
            Task.Factory.StartNew( () =>
            {
                GoodsList = GetGoods();
                fullGoodsList = GoodsList;
                FailedGoodsList = GetFailedGoods();
                MatchesList = GetMatchesList();
            } );

        }

        public void MakeMatching(object obj = null)
        {
            if (SelectedFailedGood != null && SelectedGood != null)
            {
                if (!String.IsNullOrEmpty( SelectedFailedGood.BuyerItemCode ) && !String.IsNullOrEmpty( SelectedGood.Id ))
                {
                    DbService.Insert( $@"insert into abt.REF_GOODS_MATCHING(CUSTOMER_GLN, CUSTOMER_ARTICLE, ID_GOOD, DISABLED)
values(4650093209994, '{SelectedFailedGood.BuyerItemCode}', {SelectedGood.Id}, 7)" );

                    List<OracleCommand> commands = new List<OracleCommand>();
                    foreach(var item in FailedGoodsList)
                    {

                        commands.Add( new OracleCommand()
                        {
                            CommandText = "EDI_REFRESH_DOC_DETAILS",
                            CommandType = CommandType.StoredProcedure,
                            Parameters =
                                {
                                    new OracleParameter("",OracleDbType.NVarChar, item?.EdiDocId ?? "null", ParameterDirection.Input)
                                }
                        } );
                    }

                    DbService.ExecuteCommand(commands  );

                    FailedGoodsList = GetFailedGoods();
                    MatchesList = GetMatchesList();
                }
            }
        }

        public void DisposeMatching(object obj = null)
        {
            /*MessageBox.Show($"DisposeEnabled:\t\t{DisposeEnabled}\n" +
$"LinkEnabled:\t\t{LinkEnabled}\n"+
$"NewMatchingEnabled:\t{IsNewMatchingEnabled}\n"+
$"SelectedGood:\t\t{SelectedGood?.Id ?? "null"}\n"+
$"SelectedFailedGood:\t\t{SelectedFailedGood?.BuyerItemCode??"null"}\n"+
$"SelectedMatch:\t\t{SelectedMatch?.CustomerGoodId ?? "null"}\n"+
$"NewCustomerItemCode:\t{NewCustomerItemCode ?? "null"}\n" +
nameof( DisposeEnabled ) + "          " + nameof( disposeEnabled ));*/
            if (SelectedFailedGood != null)
            {
                if (!String.IsNullOrEmpty( SelectedFailedGood.BuyerItemCode ))
                {
                    DbService.Insert( $@"update abt.REF_GOODS_MATCHING set DISABLED=1
where CUSTOMER_GLN = 4650093209994 and CUSTOMER_ARTICLE = '{SelectedFailedGood.BuyerItemCode}')" );
                    DbService.ExecuteCommand( new OracleCommand() { CommandText = "EDI_REFRESH_DOC_DETAILS", CommandType = System.Data.CommandType.StoredProcedure } );
                    FailedGoodsList = GetFailedGoods();
                    MatchesList = GetMatchesList();
                }
            }
        }

        public void FailedGoodSearch(object obj = null)
        {
            Task.Factory.StartNew( () =>
            {
                FailedGoodsList = GetFailedGoods();
                if (!String.IsNullOrEmpty( FailedGoodSearchText ))
                {
                    var searchList = FailedGoodSearchText.Split( ',' );
                    if (searchList.Count() > 0)
                    {
                        foreach (var item in searchList)
                        {
                            if (!String.IsNullOrEmpty( item ))
                            {
                                FailedGoodsList = FailedGoodsList.Where(
                                    x => (x.ItemDescription?.ToUpper()?.Contains( item?.ToUpper().TrimEnd( ' ' ).TrimStart( ' ' ) ) ?? false)
                                      || (x.OrderName?.ToUpper()?.Contains( item?.ToUpper().TrimEnd( ' ' ).TrimStart( ' ' ) ) ?? false)
                                      || (x.BuyerItemCode?.ToUpper()?.Contains( item?.ToUpper().TrimEnd( ' ' ).TrimStart( ' ' ) ) ?? false)
                                ).ToList();
                            }
                        }
                    }
                }
            });
        }

        public void MatchesSearch(object obj = null)
        {
            Task.Factory.StartNew( () =>
            {
                MatchesList = GetMatchesList();
                if (!String.IsNullOrEmpty( MatchesSearchText ))
                {
                    var searchList = MatchesSearchText.Split( ',' );
                    if (searchList.Count() > 0)
                    {
                        foreach (var item in searchList)
                        {
                            if (!String.IsNullOrEmpty( item ))
                            {
                                MatchesList = MatchesList.Where(
                                    x => (x.Name?.ToUpper()?.Contains( item?.ToUpper().TrimEnd( ' ' ).TrimStart( ' ' ) ) ?? false)
                                      || (x.GoodId?.ToUpper()?.Contains( item?.ToUpper().TrimEnd( ' ' ).TrimStart( ' ' ) ) ?? false)
                                      || (x.CustomerGoodId?.ToUpper()?.Contains( item?.ToUpper().TrimEnd( ' ' ).TrimStart( ' ' ) ) ?? false)
                                ).ToList();
                            }
                        }
                    }
                }
            } );
        }

        public void GoodSearch(object obj = null)
        {
            Task.Factory.StartNew( () =>
            {
                GoodsList = fullGoodsList;
                if (!String.IsNullOrEmpty( GoodSearchText ))
                {
                    var searchList = GoodSearchText.Split( ',' );
                    if (searchList.Count() > 0)
                    {
                        foreach (var item in searchList)
                        {
                            if (!String.IsNullOrEmpty( item ))
                            {
                                GoodsList = GoodsList.Where(
                                    x => (x.Name?.ToUpper().Trim( ' ' ).Contains( item?.ToUpper().Trim( ' ' ) ) ?? false)
                                      || (x.Id?.ToUpper().Trim( ' ' ).Contains( item?.ToUpper().Trim( ' ' ) ) ?? false)
                                      || (x.Manufacturer?.ToUpper().Trim( ' ' ).Contains( item?.ToUpper().Trim( ' ' ) ) ?? false)
                                      || (x.Code?.ToUpper().Trim( ' ' ).Contains( item?.ToUpper().Trim( ' ' ) ) ?? false)
                                ).ToList();
                            }
                        }
                    }
                }
            } );
        }

        private List<Goods> GetGoods()
        {
            var list = new List<string>();
            list.Add( SqlConfiguratorService.Sql_SelectGoods( ) );
            return DbService<Goods>.DocumentSelect( list );
        }

        private List<FailedGoods> GetFailedGoods()
        {
            var list = new List<string>();
            list.Add( SqlConfiguratorService.Sql_SelectFailedGoods() );
            return DbService<FailedGoods>.DocumentSelect( list );
        }

        private List<Matches> GetMatchesList()
        {
            var list = new List<string>();
            list.Add( SqlConfiguratorService.Sql_SelectMatches() );
            return DbService<Matches>.DocumentSelect( list );
        }

        public MatchMakerViewModel(MatchMakerView page)
        {
            _page = page;
        }

    }
}
