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
using static EdiClient.Services.Utils.Utilites;

namespace EdiClient.ViewModel
{
    public class MatchMakerViewModel : INotifyPropertyChanged
    {
        public MatchMakerViewModel()
        {
            //Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            //LoadDataCommand.Execute(null);
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
                return !String.IsNullOrEmpty(SelectedMatch?.EAN ?? "");
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
            get { return !String.IsNullOrEmpty(SelectedGood?.ID ?? "") && !String.IsNullOrEmpty(SelectedFailedGood?.EAN ?? ""); }
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
                //GoodsList = fullGoodsList.Where(x => x.BAR_CODE.Trim().ToUpper() == SelectedFailedGood.EAN.Trim().ToUpper()).ToList();
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
        
        public Command LoadDataCommand => new Command((o) =>
        {
            if (SelectedRelationship == null) { Error("Не выбран клиент"); return; }
            if (fullGoodsList == null || fullGoodsList.Count < 1)
            {
                var sql = DbService.Sqls.GET_GOODS;
                if (string.IsNullOrEmpty(sql)) { Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return; }
                fullGoodsList = DbService.DocumentSelect<Goods>(new List<string> { sql });
            }
            GoodsList = fullGoodsList;
            GetFailedGoods();
            GetMatchesList();
        });

        public Command MakeMatchingCommand => new Command((o) =>
        {
            Logger.Log($"[MakeMatchingCommand]P_CUSTOMER_GLN={SelectedRelationship.partnerIln}|P_ID_GOOD={SelectedGood.ID}|P_EAN={SelectedFailedGood.EAN}");

            if (SelectedFailedGood == null) { Error("Не выбран пункт с не сопоставленным товаром"); return; }
            if (SelectedGood == null) { Error("Не выбран пункт с товаром"); return; }
            if (String.IsNullOrEmpty(SelectedFailedGood.EAN) || String.IsNullOrEmpty(SelectedGood.ID))
            { Error("Код покупателя или идентификатор товара отсутствует"); return; }
            try
            {
                DbService.ExecuteCommand(new OracleCommand()
                {
                    Parameters =
                        {
                            new OracleParameter("P_CUSTOMER_GLN", OracleDbType.Number,SelectedRelationship.partnerIln, ParameterDirection.Input),
                            new OracleParameter("P_ID_GOOD", OracleDbType.Number, SelectedGood.ID, ParameterDirection.Input),
                            new OracleParameter("P_EAN", OracleDbType.NVarChar, SelectedFailedGood.EAN, ParameterDirection.Input),
                        },
                    CommandType = CommandType.StoredProcedure,
                    CommandText = (AppConfigHandler.conf.Schema + ".") + "EDI_MANAGER.MAKE_GOOD_LINK"
                });
            }
            catch (Exception ex) { Error(ex); }
        });

        public Command DisposeMatchingCommand => new Command((o) => 
        {
            Logger.Log($"[DisposeMatchingCommand]P_CUSTOMER_GLN={SelectedMatch.CUSTOMER_GLN}|P_EAN={SelectedMatch.EAN}");
            if (SelectedMatch == null) { Error("Не выбран пункт с сопоставлением"); return; }
            if (String.IsNullOrEmpty(SelectedMatch.EAN)) { Error("У выбранного товара отсутствует код покупателя"); return; }

            try
            {
                DbService.ExecuteCommand(new OracleCommand()
                {
                    Parameters =
                        {
                            new OracleParameter("P_CUSTOMER_GLN", OracleDbType.Number,SelectedMatch.CUSTOMER_GLN, ParameterDirection.Input),
                            new OracleParameter("P_EAN", OracleDbType.NVarChar, SelectedMatch.EAN, ParameterDirection.Input)
                        },
                    CommandType = CommandType.StoredProcedure,
                    CommandText = (AppConfigHandler.conf.Schema + ".") + "EDI_MANAGER.MAKE_GOOD_UNLINK"
                });

                GetFailedGoods();
                GetMatchesList();
            }
            catch (Exception ex) { Error(ex); }
        });

        #endregion
        
        protected void RaiseNotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private void GetFailedGoods()
        {
            FailedGoodsList = DbService.DocumentSelect<FailedGoods>(new List<string> { DbService.Sqls.GET_FAILED_DETAILS(SelectedRelationship?.partnerIln) });
        }

        private void GetMatchesList()
        {
            MatchesList = DbService.DocumentSelect<Matches>(new List<string> { DbService.Sqls.GET_MATCHED(SelectedRelationship?.partnerIln) });
        }

    }
}