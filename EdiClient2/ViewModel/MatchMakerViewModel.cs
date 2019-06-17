using Devart.Data.Oracle;
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
using System.Windows.Controls;
using System.Windows.Media;
using EdiClient.AppSettings;

namespace EdiClient.ViewModel
{
    public class MatchMakerViewModel : INotifyPropertyChanged
    {
        public MatchMakerViewModel(MatchMakerView page)
        {
            try
            {
                _page = page;
            }
            catch (Exception ex) { Utilites.Error(ex); }
        }

        #region fields
        
        public List<Model.WebModel.RelationResponse.Relation> Relationships => EdiService.Relationships;        
        public Model.WebModel.RelationResponse.Relation SelectedRelationship => EdiService.SelectedRelationship;
        public int RelationshipCount => EdiService.RelationshipCount;

        private MatchMakerView _page { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isNewMatchingEnabled = true;
        public virtual bool IsNewMatchingEnabled
        {
            get
            {
                return !String.IsNullOrEmpty(NewCustomerItemCode);/* || (NewCustomerItemCode?.Length ?? 0) >= 4 ) && SelectedGood != null*/;
            }
            set
            {
                isNewMatchingEnabled = value;
                RaiseNotifyPropertyChanged("IsNewMatchingEnabled");
            }
        }


        private bool disposeEnabled = true;
        public bool DisposeEnabled
        {
            get
            {
                return !String.IsNullOrEmpty(SelectedMatch?.CUSTOMER_ARTICLE ?? "");
            }
            set
            {
                disposeEnabled = value;
                RaiseNotifyPropertyChanged("DisposeEnabled");
            }
        }

        private bool linkEnabled = true;
        public bool LinkEnabled
        {
            get { return !String.IsNullOrEmpty(SelectedGood?.ID ?? "") && !String.IsNullOrEmpty(SelectedFailedGood?.BUYER_ITEM_CODE ?? ""); }
            set
            {
                linkEnabled = value;
                RaiseNotifyPropertyChanged("LinkEnabled");
            }
        }

        private Goods selectedGood = new Goods();
        public Goods SelectedGood
        {
            get { return selectedGood; }
            set
            {
                selectedGood = value;
                RaiseNotifyPropertyChanged("SelectedGood");
            }
        }

        private FailedGoods selectedFailedGood = new FailedGoods();
        public FailedGoods SelectedFailedGood
        {
            get { return selectedFailedGood; }
            set
            {
                selectedFailedGood = value;
                RaiseNotifyPropertyChanged("SelectedFailedGood");
                GoodsList = fullGoodsList.Where(x => x.BAR_CODE.Trim().ToUpper() == SelectedFailedGood.EAN.Trim().ToUpper()).ToList();
            }
        }

        private Matches selectedMatch = new Matches();
        public Matches SelectedMatch
        {
            get { return selectedMatch; }
            set
            {
                selectedMatch = value;
                RaiseNotifyPropertyChanged("SelectedMatch");
            }
        }


        private string newCustomerItemCode = "";
        public string NewCustomerItemCode
        {
            get { return newCustomerItemCode; }
            set
            {
                newCustomerItemCode = value;
                RaiseNotifyPropertyChanged("NewCustomerItemCode");
            }
        }

        private string goodSearchText = "";
        public string GoodSearchText
        {
            get { return goodSearchText; }
            set
            {
                goodSearchText = value;
                RaiseNotifyPropertyChanged("GoodSearchText");
            }
        }

        private string failedGoodSearchText = "";
        public string FailedGoodSearchText
        {
            get { return failedGoodSearchText; }
            set
            {
                failedGoodSearchText = value;
                RaiseNotifyPropertyChanged("FailedGoodSearchText");
            }
        }

        private string matchesSearchText = "";
        public string MatchesSearchText
        {
            get { return matchesSearchText; }
            set
            {
                matchesSearchText = value;
                RaiseNotifyPropertyChanged("MatchesSearchText");
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
                RaiseNotifyPropertyChanged("GoodsList");
            }
        }

        private List<FailedGoods> failedGoodsList = new List<FailedGoods>();
        public List<FailedGoods> FailedGoodsList
        {
            get { return failedGoodsList; }
            set
            {
                failedGoodsList = value;
                RaiseNotifyPropertyChanged("FailedGoodsList");
            }
        }

        private List<Matches> matchesList = new List<Matches>();
        public List<Matches> MatchesList
        {
            get { return matchesList; }
            set
            {
                matchesList = value;
                RaiseNotifyPropertyChanged("MatchesList");
            }
        }

        private bool HelpMode { get; set; } = false;

        //public CommandService NewMatchingCommand => new CommandService(NewMatching);
        public CommandService FailedGoodSearchCommand => new CommandService(FailedGoodSearch);
        public CommandService MatchesSearchCommand => new CommandService(MatchesSearch);
        public CommandService GoodSearchCommand => new CommandService(GoodSearch);
        public CommandService LoadDataCommand => new CommandService(LoadData);
        public CommandService MakeMatchingCommand => new CommandService(MakeMatching);
        public CommandService DisposeMatchingCommand => new CommandService(DisposeMatching);
        public CommandService FailedGoodResetInputCommand => new CommandService(FailedGoodResetInput);
        public CommandService MatchesResetInputCommand => new CommandService(MatchesResetInput);
        public CommandService GoodResetInputCommand => new CommandService(GoodResetInput);

        #endregion
        
        protected void RaiseNotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        
        private void SetLayoutEnabled(bool flag)
        {
            _page.LoadDataButton.IsEnabled = flag;
            _page.LoadDataButton.UpdateLayout();
        }

        public void LoadData(object obj = null)
        {
            SetLayoutEnabled(false);
            if(fullGoodsList == null || fullGoodsList.Count < 1)
            {
                fullGoodsList = GetGoods();
            }
            GoodsList = fullGoodsList;
            FailedGoodsList = GetFailedGoods();
            MatchesList = GetMatchesList();
            
            SetLayoutEnabled(true);
        }

        public void MakeMatching(object obj = null)
        {
            if (SelectedFailedGood == null) { Utilites.Error("Не выбран пункт с не сопоставленным товаром"); return; }
            if (SelectedGood == null) { Utilites.Error("Не выбран пункт с товаром"); return; }
            if (String.IsNullOrEmpty(SelectedFailedGood.BUYER_ITEM_CODE) || String.IsNullOrEmpty(SelectedGood.ID)) 
                { Utilites.Error("Код покупателя или идентификатор товара отсутствует"); return; }

            DbService.ExecuteCommand(new OracleCommand()
            {
                Parameters =
                        {
                            new OracleParameter("P_CUSTOMER_GLN", OracleDbType.Number,SelectedRelationship.partnerIln, ParameterDirection.Input),
                            new OracleParameter("P_CUSTOMER_ARTICLE", OracleDbType.NVarChar, SelectedFailedGood.BUYER_ITEM_CODE, ParameterDirection.Input),
                            new OracleParameter("P_ID_GOOD", OracleDbType.Number, SelectedGood.ID, ParameterDirection.Input),
                            new OracleParameter("P_ID_EDI_DOC", OracleDbType.Number, SelectedFailedGood.ID_EDI_DOC, ParameterDirection.Input),
                        },
                Connection = OracleConnectionService.conn,
                CommandType = CommandType.StoredProcedure,
                CommandText = AppConfig.Schema + "EDI_MAKE_GOOD_LINK"
            });
        }

        public void DisposeMatching(object obj = null)
        {
            if (SelectedMatch == null) { Utilites.Error("Не выбран пункт с сопоставлением"); return; }
            if (String.IsNullOrEmpty(SelectedMatch.CUSTOMER_ARTICLE)) { Utilites.Error("У выбранного товара отсутствует код покупателя"); return; }

            try
            {
                DbService.ExecuteCommand(new OracleCommand()
                {
                    Parameters =
                        {
                            new OracleParameter("P_CUSTOMER_GLN", OracleDbType.Number,SelectedMatch.CUSTOMER_GLN, ParameterDirection.Input),
                            new OracleParameter("P_CUSTOMER_ARTICLE", OracleDbType.NVarChar, SelectedMatch.CUSTOMER_ARTICLE, ParameterDirection.Input),
                            new OracleParameter("P_ID_EDI_DOC", OracleDbType.Number, SelectedFailedGood.ID_EDI_DOC, ParameterDirection.Input),
                        },
                    Connection = OracleConnectionService.conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = AppConfig.Schema + "EDI_MAKE_GOOD_UNLINK"
                });
                
                FailedGoodsList = GetFailedGoods();
                MatchesList = GetMatchesList();
            }
            catch (Exception ex) { Utilites.Error(ex); }
        }
        

        public void FailedGoodResetInput(object obj = null) => FailedGoodsList = GetFailedGoods();
        public void MatchesResetInput(object obj = null) => MatchesList = GetMatchesList();
        public void GoodResetInput(object obj = null) => GoodsList = GetGoods();


        public void FailedGoodSearch(object obj = null) => Task.Factory.StartNew(() =>
           {
               FailedGoodResetInput();
               if (!String.IsNullOrEmpty(FailedGoodSearchText))
               {
                   var searchList = FailedGoodSearchText.Split(' ');
                   if (searchList.Count() > 0)
                       foreach (var item in searchList)
                       {
                           var text = item?.ToUpper()?.Trim(' ') ?? "";
                           if (!String.IsNullOrEmpty(item))
                               FailedGoodsList = FailedGoodsList.Where(
                                   x => (x.ITEM_DESCRIPTION?.ToUpper()?.Contains(text) ?? false)
                                     || (x.ORDER_NUMBER?.ToUpper()?.Contains(text) ?? false)
                                     || (x.ORDER_DATE?.ToUpper()?.Contains(text) ?? false)
                                     || (x.BUYER_ITEM_CODE?.ToUpper()?.Contains(text) ?? false)
                                     || (x.EAN?.ToUpper()?.Contains(text) ?? false)
                               ).ToList();
                       }
               }
           });


        public void MatchesSearch(object obj = null) => Task.Factory.StartNew(() =>
           {
               MatchesResetInput();
               if (!String.IsNullOrEmpty(MatchesSearchText))
               {
                   var searchList = MatchesSearchText.Split(' ');
                   if (searchList.Count() > 0)
                       foreach (var item in searchList)
                           if (!String.IsNullOrEmpty(item))
                           {
                               var text = item?.ToUpper()?.Trim(' ') ?? "";
                               MatchesList = MatchesList.Where(
                                   x => (x.NAME?.ToUpper()?.Contains(text) ?? false)
                                     || (x.ID_GOOD?.ToUpper()?.Contains(text) ?? false)
                                     || (x.CUSTOMER_ARTICLE?.ToUpper()?.Contains(text) ?? false)
                                     || (x.BAR_CODE?.ToUpper()?.Contains(text) ?? false)
                                     || (x.INSERT_DATETIME?.ToUpper()?.Contains(text) ?? false)
                               ).ToList();
                           }
               }
           });


        public void GoodSearch(object obj = null) => Task.Factory.StartNew(() =>
           {
               GoodsList = fullGoodsList;
               if (!String.IsNullOrEmpty(GoodSearchText))
               {
                   var searchList = GoodSearchText.Split(' ');
                   if (searchList.Count() > 0)
                       foreach (var item in searchList)
                           if (!String.IsNullOrEmpty(item))
                           {
                               var text = item?.ToUpper()?.Trim(' ') ?? "";
                               GoodsList = GoodsList.Where(
                                   x => (x.NAME?.ToUpper().Trim(' ').Contains(text) ?? false)
                                     || (x.ID?.ToUpper().Trim(' ').Contains(text) ?? false)
                                     || (x.MANUFACTURER?.ToUpper().Trim(' ').Contains(text) ?? false)
                                     || (x.CODE?.ToUpper().Trim(' ').Contains(text) ?? false)
                                     || (x.BAR_CODE?.ToUpper().Trim(' ').Contains(text) ?? false)
                               ).ToList();
                           }
               }
           });



        private List<Goods> GetGoods()
        {
            var sql = SqlService.GET_GOODS;
            if (string.IsNullOrEmpty(sql)) { Utilites.Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService<Goods>.DocumentSelect(new List<string> { sql });
            return result;
        }
        

        private List<FailedGoods> GetFailedGoods()
        {
            if (SelectedRelationship == null) { Utilites.Error("Не выбран клиент"); return null; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Не выбран клиент"); return null; }
            var sql = SqlService.GET_FAILED_DETAILS(SelectedRelationship?.partnerIln);
            if (string.IsNullOrEmpty(sql)) { Utilites.Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService<FailedGoods>.DocumentSelect(new List<string> { sql });
            return result;
        }

        private List<Matches> GetMatchesList()
        {
            if (SelectedRelationship == null) { Utilites.Error("Не выбран клиент"); return null; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Не выбран клиент"); return null; }
            var sql = SqlService.GET_MATCHED(SelectedRelationship?.partnerIln);
            if (string.IsNullOrEmpty(sql)) { Utilites.Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService<Matches>.DocumentSelect(new List<string> { sql });
            return result;
        }

    }
}
