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
            catch (Exception ex) { err(ex); }
        }

        #region fields

        delegate void Message(string ex);
        Message msg = Utilites.Error;
        delegate void Error(Exception ex);
        Error err = Utilites.Error;

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
                return !String.IsNullOrEmpty(SelectedMatch?.CustomerGoodId ?? "");
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
            get { return !String.IsNullOrEmpty(SelectedGood?.Id ?? "") && !String.IsNullOrEmpty(SelectedFailedGood?.BuyerItemCode ?? ""); }
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

        public CommandService NewMatchingCommand => new CommandService(NewMatching);
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

        public void NewMatching(object obj = null)
        {


            if (NewCustomerItemCode.Length < 4 || NewCustomerItemCode == null)
            {
                Utilites.Error("Поле с кодом не должно быть короче 4 символов и быть пустым");
                return;
            }

            if (SelectedRelationship == null)
            {
                Utilites.Error("Необходимо выбрать покупателя");
                return;
            }

            if (String.IsNullOrEmpty(SelectedRelationship.partnerIln))
            {
                Utilites.Error("У выбранного покупателя нет кода ГЛН");
                return;
            }

            if (SelectedGood == null)
            {
                Utilites.Error("Необходимо выбрать товар для сопоставления");
                return;
            }

            if (String.IsNullOrEmpty(SelectedGood.Id))
            {
                Utilites.Error("Товар имеет не верный идентификатор");
                return;
            }

            DbService.Insert($@"insert into abt.REF_GOODS_MATCHING(CUSTOMER_GLN,CUSTOMER_ARTICLE,ID_GOOD,DISABLED)
values('{SelectedRelationship.partnerIln}',{NewCustomerItemCode},{SelectedGood.Id},0)");

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
            if (EdiService.SelectedRelationship == null) { Utilites.Error("Необходимо выбрать клиента"); return; }
            SetLayoutEnabled(false);
            _page.LoadDataButton.Content = "ждите загрузки";

            GoodsList = GetGoods();
            fullGoodsList = GoodsList;
            FailedGoodsList = GetFailedGoods();
            MatchesList = GetMatchesList();

            _page.LoadDataButton.Content = "Загрузить данные";
            SetLayoutEnabled(true);

        }

        public void MakeMatching(object obj = null)
        {
            if (SelectedFailedGood == null)
            {
                msg("Не выбран пункт с не сопоставленным товаром");
                return;
            }

            if (SelectedGood == null)
            {
                msg("Не выбран пункт с товаром");
                return;
            }

            if (String.IsNullOrEmpty(SelectedFailedGood.BuyerItemCode) || String.IsNullOrEmpty(SelectedGood.Id))
            {
                msg("Код покупателя или идентификатор товара отсутствует");
                return;
            }


            var sql = $"insert into abt.REF_GOODS_MATCHING(CUSTOMER_GLN, CUSTOMER_ARTICLE, ID_GOOD, DISABLED)" +
                $"values({SelectedRelationship.partnerIln}, '{SelectedFailedGood.BuyerItemCode}', {SelectedGood.Id}, 0)";

            DbService.Insert(sql);

            UpdateDocs(SelectedFailedGood.EdiDocId);
        }


        public void UpdateDocs(string EdiDocId)
        {
            try
            {
                DbService.ExecuteCommand(new OracleCommand()
                {
                    CommandText = "EDI_REFRESH_DOC_DETAILS",
                    CommandType = CommandType.StoredProcedure,
                    Parameters =
                    {
                        new OracleParameter("P_EDI_DOC_ID", OracleDbType.NVarChar, EdiDocId, ParameterDirection.Input)
                    }
                });
                FailedGoodsList = GetFailedGoods();
                MatchesList = GetMatchesList();
            }
            catch (Exception ex) { err(ex); }
        }


        public void DisposeMatching(object obj = null)
        {
            if (SelectedMatch == null)
            {
                msg("Не выбран пункт с сопоставлением");
                return;
            }

            if (String.IsNullOrEmpty(SelectedMatch.CustomerGoodId))
            {
                msg("У выбранного товара отсутствует код покупателя");
                return;
            }

            try
            {
                DbService.Insert($@"update abt.REF_GOODS_MATCHING set DISABLED=1
where CUSTOMER_GLN = {SelectedMatch.CustomerGln} and CUSTOMER_ARTICLE = '{SelectedMatch.CustomerGoodId}'");

                FailedGoodsList = GetFailedGoods();
                MatchesList = GetMatchesList();

                DbService.Insert($"DECLARE CURSOR v_cursor IS SELECT UNIQUE ID_EDI_DOC FROM HPCSERVICE.EDI_DOC_DETAILS WHERE ID_GOOD = {SelectedMatch.GoodId};" +
                "BEGIN FOR DOC IN v_cursor LOOP HPCSERVICE.EDI_REFRESH_DOC_DETAILS(DOC.ID_EDI_DOC); END LOOP; EXCEPTION WHEN OTHERS THEN NULL; END;");
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
                                   x => (x.ItemDescription?.ToUpper()?.Contains(text) ?? false)
                                     || (x.OrderName?.ToUpper()?.Contains(text) ?? false)
                                     || (x.BuyerItemCode?.ToUpper()?.Contains(text) ?? false)
                                     || (x.Ean?.ToUpper()?.Contains(text) ?? false)
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
                                   x => (x.Name?.ToUpper()?.Contains(text) ?? false)
                                     || (x.GoodId?.ToUpper()?.Contains(text) ?? false)
                                     || (x.CustomerGoodId?.ToUpper()?.Contains(text) ?? false)
                                     || (x.BarCode?.ToUpper()?.Contains(text) ?? false)
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
                                   x => (x.Name?.ToUpper().Trim(' ').Contains(text) ?? false)
                                     || (x.Id?.ToUpper().Trim(' ').Contains(text) ?? false)
                                     || (x.Manufacturer?.ToUpper().Trim(' ').Contains(text) ?? false)
                                     || (x.Code?.ToUpper().Trim(' ').Contains(text) ?? false)
                                     || (x.BarCode?.ToUpper().Trim(' ').Contains(text) ?? false)
                               ).ToList();
                           }
               }
           });



        private List<Goods> GetGoods() 
            => DbService<Goods>.DocumentSelect(new List<string> { SqlConfiguratorService.Sql_SelectGoods() });

        private List<FailedGoods> GetFailedGoods() 
            => DbService<FailedGoods>.DocumentSelect(new List<string> { SqlConfiguratorService.Sql_SelectFailedGoods() });

        private List<Matches> GetMatchesList() 
            => DbService<Matches>.DocumentSelect(new List<string> { SqlConfiguratorService.Sql_SelectMatches() })
            .Where(x => x.CustomerGln == SelectedRelationship.partnerIln)
            .ToList();

    }
}
