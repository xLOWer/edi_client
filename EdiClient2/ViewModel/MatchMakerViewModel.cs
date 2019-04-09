﻿using Devart.Data.Oracle;
using EdiClient.Model.MatchingDbModel;
using EdiClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using EdiClient.View;

namespace EdiClient.ViewModel
{
    public class MatchMakerViewModel : INotifyPropertyChanged
    {
        #region fields

        private MatchMakerView _page { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

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
        public CommandService FailedGoodResetInputCommand => new CommandService( FailedGoodResetInput );
        public CommandService MatchesResetInputCommand => new CommandService( MatchesResetInput );
        public CommandService GoodResetInputCommand => new CommandService( GoodResetInput );

        #endregion

        protected void RaiseNotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( info ) );
        }


        
        public void NewMatching(object obj = null)
        {

            if (NewCustomerItemCode.Length < 4 || NewCustomerItemCode == null)
            {
                Utilites.Error( "Поле с кодом не должно быть короче 4 символов и быть пустым" );
                return;
            }

            if(SelectedGood == null)
            {
                Utilites.Error( "Необходимо выбрать товар для сопоставления" );
                return;
            }

            if ( String.IsNullOrEmpty( SelectedGood.Id ))
            {
                Utilites.Error( "Товар имеет не верный идентификатор" );
                return;
            }

            DbService.Insert( $@"insert abt.REF_GOODS_MATCHING(CUSTOMER_GLN,CUSTOMER_ARTICLE,ID_GOOD,DISABLED)
values('4650093209994',{NewCustomerItemCode},{SelectedGood.Id},7)" );

            NewCustomerItemCode = "";

            FailedGoodsList = GetFailedGoods();
            MatchesList = GetMatchesList();
        }
        
        private void SetLayoutEnabled(bool flag)
        {
            _page.LoadDataButton.IsEnabled = flag;
            _page.LoadDataButton.UpdateLayout();
        }
        
        public void LoadData(object obj = null)
        {
            SetLayoutEnabled( false );
            _page.LoadDataButton.Content = "ждите загрузки";

           GoodsList = GetGoods();
            fullGoodsList = GoodsList;
            FailedGoodsList = GetFailedGoods();
            MatchesList = GetMatchesList();

            _page.LoadDataButton.Content = "Загрузить данные";
            SetLayoutEnabled( true );

        }
        
        public void MakeMatching(object obj = null)
        {
            if (SelectedFailedGood == null)
            {
                Utilites.Error( "Не выбран пункт с не сопоставленным товаром" );
                return;
            }

            if (SelectedGood == null)
            {
                Utilites.Error( "Не выбран пункт с товаром" );
                return;
            }

            if (String.IsNullOrEmpty( SelectedFailedGood.BuyerItemCode ) || String.IsNullOrEmpty( SelectedGood.Id ))
            {
                Utilites.Error( "Код покупателя или идентификатор товара отсутствует" );
                return;
            }
            var sql = $@"insert into abt.REF_GOODS_MATCHING(CUSTOMER_GLN, CUSTOMER_ARTICLE, ID_GOOD, DISABLED)
values(4650093209994, '{SelectedFailedGood.BuyerItemCode}', {SelectedGood.Id}, 0)";
            MessageBox.Show(sql);
            DbService.Insert( sql  );

            string failed = "";
            List<OracleCommand> commands = new List<OracleCommand>();

            foreach (var item in FailedGoodsList)
            {
                if (item == null)
                {
                    failed += $"\n{item.GetType().ToString()} is null in {nameof( FailedGoodsList )}";
                    break;
                }

                if (!String.IsNullOrEmpty( item.EdiDocId ))
                {
                    failed += $"\n{item.GetType().ToString()}.EdiDocId is null of empty in {nameof( FailedGoodsList )}";
                    break;
                }

                commands.Add( new OracleCommand()
                {
                    CommandText = "EDI_REFRESH_DOC_DETAILS",
                    CommandType = CommandType.StoredProcedure,
                    Parameters =
                            {
                                new OracleParameter("P_EDI_DOC_NUMBER", OracleDbType.NVarChar, item.EdiDocId, ParameterDirection.Input)
                            }
                } );
            }
            if (commands.Count <= 0)
            {
                Utilites.Error( "Нет команд для выполнения т.к. возможно имеются отшибки в выделяемых товарах: "+failed );
                return;
            }
            try
            {
                DbService.ExecuteCommand( commands );
                FailedGoodsList = GetFailedGoods();
                MatchesList = GetMatchesList();
            }
            catch (Exception ex) { Utilites.Error( ex ); }
        }
        
        public void DisposeMatching(object obj = null)
        {
            if (SelectedFailedGood == null)
            {
                Utilites.Error( "Не выбран пункт с несопоставленным товаром" );

            }

            if (String.IsNullOrEmpty( SelectedFailedGood.BuyerItemCode ))
            {
                Utilites.Error( "У выбранного товара отсутствует код покупателя" );
            }

            try
            {
                DbService.Insert( $@"update abt.REF_GOODS_MATCHING set DISABLED=1
where CUSTOMER_GLN = 4650093209994 and CUSTOMER_ARTICLE = '{SelectedFailedGood.BuyerItemCode}')" );

                DbService.ExecuteCommand( new OracleCommand()
                {
                    CommandText = "EDI_REFRESH_DOC_DETAILS",
                    CommandType = CommandType.StoredProcedure
                } );
                FailedGoodsList = GetFailedGoods();
                MatchesList = GetMatchesList();
            }
            catch (Exception ex) { Utilites.Error( ex ); }

        }



        public void FailedGoodResetInput(object obj = null)
        {
            FailedGoodSearch( "" );
        }

        public void MatchesResetInput(object obj = null)
        {
            MatchesSearch( "" );
        }

        public void GoodResetInput(object obj = null)
        {
            GoodSearch( "" );
        }

        public void FailedGoodSearch(object obj = null)
        {
            if (obj == "")
            {
                FailedGoodsList = GetFailedGoods();
                return;
            }

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
                                    x => (x.ItemDescription?.ToUpper()?.Contains( item?.ToUpper().Trim( ' ' ) ) ?? false)
                                      || (x.OrderName?.ToUpper()?.Contains( item?.ToUpper().Trim( ' ' ) ) ?? false)
                                      || (x.BuyerItemCode?.ToUpper()?.Contains( item?.ToUpper().Trim( ' ' ) ) ?? false)
                                ).ToList();
                            }
                        }
                    }
                }
            });
        }
        
        public void MatchesSearch(object obj = null)
        {
            if (obj == "")
            {
                MatchesList = GetMatchesList();
                return;
            }

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
                                    x => (x.Name?.ToUpper()?.Contains( item?.ToUpper().Trim( ' ' ) ) ?? false)
                                      || (x.GoodId?.ToUpper()?.Contains( item?.ToUpper().Trim( ' ' ) ) ?? false)
                                      || (x.CustomerGoodId?.ToUpper()?.Contains( item?.ToUpper().Trim( ' ' ) ) ?? false)
                                ).ToList();
                            }
                        }
                    }
                }
            } );
        }
        
        public void GoodSearch(object obj = null)
        {
            if (obj == "")
            {
                GoodsList = fullGoodsList;
                return;
            }

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
            var list = new List<string>
            {
                SqlConfiguratorService.Sql_SelectGoods()
            };
            return DbService<Goods>.DocumentSelect( list );
        }
        
        private List<FailedGoods> GetFailedGoods()
        {
            var list = new List<string>
            {
                SqlConfiguratorService.Sql_SelectFailedGoods()
            };
            return DbService<FailedGoods>.DocumentSelect( list );
        }
        
        private List<Matches> GetMatchesList()
        {
            var list = new List<string>
            {
                SqlConfiguratorService.Sql_SelectMatches()
            };
            return DbService<Matches>.DocumentSelect( list );
        }


        
        public MatchMakerViewModel(MatchMakerView page)
        {
            _page = page;
        }


    }
}