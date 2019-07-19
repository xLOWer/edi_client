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
using System.Reflection;

namespace EdiClient.ViewModel
{
    public class MatchMakerViewModel : INotifyPropertyChanged
    {
        public MatchMakerViewModel(MatchMakerView page)
        {
            EdiClient.Services.LogService.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
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
        
        public CommandService LoadDataCommand => new CommandService(LoadData);
        public CommandService MakeMatchingCommand => new CommandService(MakeMatching);
        public CommandService DisposeMatchingCommand => new CommandService(DisposeMatching);

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
            LogService.Log($"[GOODS] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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
            LogService.Log($"[GOODS] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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
                        },
                Connection = OracleConnectionService.conn,
                CommandType = CommandType.StoredProcedure,
                CommandText = (AppConfig.Schema + ".") + "EDI_MAKE_GOOD_LINK"
            });
        }

        public void DisposeMatching(object obj = null)
        {
            LogService.Log($"[GOODS] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedMatch == null) { Utilites.Error("Не выбран пункт с сопоставлением"); return; }
            if (String.IsNullOrEmpty(SelectedMatch.CUSTOMER_ARTICLE)) { Utilites.Error("У выбранного товара отсутствует код покупателя"); return; }

            try
            {
                DbService.ExecuteCommand(new OracleCommand()
                {
                    Parameters =
                        {
                            new OracleParameter("P_CUSTOMER_GLN", OracleDbType.Number,SelectedMatch.CUSTOMER_GLN, ParameterDirection.Input),
                            new OracleParameter("P_CUSTOMER_ARTICLE", OracleDbType.NVarChar, SelectedMatch.CUSTOMER_ARTICLE, ParameterDirection.Input)
                        },
                    Connection = OracleConnectionService.conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = (AppConfig.Schema + ".") + "EDI_MAKE_GOOD_UNLINK"
                });
                
                FailedGoodsList = GetFailedGoods();
                MatchesList = GetMatchesList();
            }
            catch (Exception ex) { Utilites.Error(ex); }
        }
                
        private List<Goods> GetGoods()
        {
            LogService.Log($"[GOODS] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            var sql = SqlService.GET_GOODS;
            if (string.IsNullOrEmpty(sql)) { Utilites.Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService<Goods>.DocumentSelect(new List<string> { sql });
            return result;
        }
        

        private List<FailedGoods> GetFailedGoods()
        {
            LogService.Log($"[GOODS] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedRelationship == null) { Utilites.Error("Не выбран клиент"); return null; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Не выбран клиент"); return null; }
            var sql = SqlService.GET_FAILED_DETAILS(SelectedRelationship?.partnerIln);
            if (string.IsNullOrEmpty(sql)) { Utilites.Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService<FailedGoods>.DocumentSelect(new List<string> { sql });
            return result;
        }

        private List<Matches> GetMatchesList()
        {
            LogService.Log($"[GOODS] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedRelationship == null) { Utilites.Error("Не выбран клиент"); return null; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Не выбран клиент"); return null; }
            var sql = SqlService.GET_MATCHED(SelectedRelationship?.partnerIln);
            if (string.IsNullOrEmpty(sql)) { Utilites.Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService<Matches>.DocumentSelect(new List<string> { sql });
            return result;
        }

    }
}