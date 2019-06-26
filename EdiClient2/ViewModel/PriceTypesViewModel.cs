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
using static EdiClient.Model.WebModel.RelationResponse;
using EdiClient.AppSettings;
using System.Reflection;

namespace EdiClient.ViewModel
{
    public class PriceTypesViewModel : INotifyPropertyChanged
    {
        public PriceTypesViewModel(PriceTypesView page)
        {
            try
            {
                _page = page;
            } catch (Exception ex) { Utilites.Error( ex ); }
        }

        #region fields
                
        public List<Relation> Relationships => EdiService.Relationships;
        public Relation SelectedRelationship => EdiService.SelectedRelationship;
        public int RelationshipCount => EdiService.RelationshipCount;

        private PriceTypesView _page { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private PriceType selectedPriceType = new PriceType();
        public PriceType SelectedPriceType
        {
            get { return selectedPriceType; }
            set
            {
                selectedPriceType = value;
                RaiseNotifyPropertyChanged( "SelectedPriceType" );
            }
        }
        
        private MatchingPriceTypes selectedMatch = new MatchingPriceTypes();
        public MatchingPriceTypes SelectedMatch
        {
            get { return selectedMatch; }
            set
            {
                selectedMatch = value;
                RaiseNotifyPropertyChanged( "SelectedMatch" );
            }
        }
          

        private string priceTypeSearchText = "";
        public string PriceTypeSearchText
        {
            get { return priceTypeSearchText; }
            set
            {
                priceTypeSearchText = value;
                RaiseNotifyPropertyChanged( "PriceTypeSearchText" );
            }
        }
        
        private string matchSearchText = "";
        public string MatchSearchText
        {
            get { return matchSearchText; }
            set
            {
                matchSearchText = value;
                RaiseNotifyPropertyChanged( "MatchSearchText" );
            }
        }
        
        private List<PriceType> priceTypeList = new List<PriceType>();
        public List<PriceType> PriceTypeList
        {
            get { return priceTypeList; }
            set
            {
                priceTypeList = value;
                RaiseNotifyPropertyChanged( "PriceTypeList" );
            }
        }
        
        private List<MatchingPriceTypes> matchList = new List<MatchingPriceTypes>();
        public List<MatchingPriceTypes> MatchList
        {
            get { return matchList; }
            set
            {
                matchList = value;
                RaiseNotifyPropertyChanged( "MatchList" );
            }
        }

        private bool HelpMode { get; set; } = false;
        
        public CommandService MatchSearchCommand => new CommandService( MatchSearch );
        public CommandService PriceTypeSearchCommand => new CommandService( PriceTypeSearch );
        public CommandService LoadDataCommand => new CommandService( LoadData );
        public CommandService MakeMatchingCommand => new CommandService( MakeMatching );
        public CommandService DisposeMatchingCommand => new CommandService( DisposeMatching );
        public CommandService MatchResetInputCommand => new CommandService( MatchResetInput );
        public CommandService PriceTypesResetInputCommand => new CommandService( PriceTypesResetInput );

        #endregion

        protected void RaiseNotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( info ) );
        }            
        
        private void SetLayoutEnabled(bool flag)
        {
            _page.LoadDataButton.IsEnabled = flag;
            _page.LoadDataButton.UpdateLayout();
        }
        
        public void LoadData(object obj = null)
        {
            SetLayoutEnabled( false );

           PriceTypeList = GetPriceTypes();
            MatchList = GetMatchList();
            
            SetLayoutEnabled( true );

        }
        
        public void MakeMatching(object obj = null)
        {
            LogService.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedRelationship == null) { Utilites.Error( "Не выбран покупатель" ); return; }
            if (SelectedPriceType == null) { Utilites.Error( "Не выбран тип цены" ); return; }

            try
            {
                DbService.ExecuteCommand(new OracleCommand()
                {
                    Parameters =
                        {
                            new OracleParameter("P_CUSTOMER_GLN", OracleDbType.Number,SelectedMatch.CUSTOMER_GLN, ParameterDirection.Input),
                            new OracleParameter("ID_PRICE_TYPE", OracleDbType.NVarChar, SelectedMatch.ID_PRICE_TYPE, ParameterDirection.Input),
                        },
                    Connection = OracleConnectionService.conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = (AppConfig.Schema + ".") + "EDI_MAKE_PRICE_LINK"
                });

                MatchList = GetMatchList();
            }
            catch (Exception ex) { Utilites.Error( ex ); }
        }
        
        
        public void DisposeMatching(object obj = null)
        {
            LogService.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedMatch == null) { Utilites.Error( "Не выбран пункт с сопоставлением" ); return; }
            
            try
            {
                DbService.ExecuteCommand(new OracleCommand()
                {
                    Parameters =
                        {
                            new OracleParameter("P_CUSTOMER_GLN", OracleDbType.Number,SelectedMatch.CUSTOMER_GLN, ParameterDirection.Input),
                            new OracleParameter("ID_PRICE_TYPE", OracleDbType.NVarChar, SelectedMatch.ID_PRICE_TYPE, ParameterDirection.Input),
                        },
                    Connection = OracleConnectionService.conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = (AppConfig.Schema + ".") + "EDI_MAKE_PRICE_UNLINK"
                });
                MatchList = GetMatchList();
            }
            catch (Exception ex) { Utilites.Error( ex ); }
        }
                        

        public void MatchSearch(object obj = null) =>
            Task.Factory.StartNew( () =>
            {
                LogService.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
                MatchResetInput();
                if (!String.IsNullOrEmpty(MatchSearchText))
                {
                    var searchList = MatchSearchText.Split(' ');
                    if (searchList.Count() > 0)
                        foreach (var item in searchList)
                            if (!String.IsNullOrEmpty(item))
                            {
                                var text = item?.ToUpper()?.Trim(' ') ?? "";
                                MatchList = MatchList.Where(
                                    x => (x.CUSTOMER_GLN?.ToUpper()?.Contains(text) ?? false)
                                      || (x.ID_PRICE_TYPE?.ToUpper()?.Contains(text) ?? false)
                                      || (x.INSERT_DATETIME?.ToUpper()?.Contains(text) ?? false)
                                      || (x.NAME?.ToUpper()?.Contains(text) ?? false)
                                ).ToList();
                            }
                }
            } );
        

        public void PriceTypeSearch(object obj = null) =>        
            Task.Factory.StartNew( () =>
            {
                LogService.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
                PriceTypesResetInput();
                if (!String.IsNullOrEmpty( PriceTypeSearchText ))
                {
                    var searchList = PriceTypeSearchText.Split( ' ' );
                    if (searchList.Count() > 0)                    
                        foreach (var item in searchList)                        
                            if (!String.IsNullOrEmpty( item ))
                            {
                                var text = item?.ToUpper()?.Trim( ' ' ) ?? "";
                                PriceTypeList = PriceTypeList.Where(
                                    x => (x.NAME?.ToUpper().Trim( ' ' ).Contains( text ) ?? false)
                                      || (x.ID?.ToUpper().Trim( ' ' ).Contains( text ) ?? false)
                                ).ToList();
                            }
                }
            } );

        
        public void MatchResetInput(object obj = null) => MatchList = GetMatchList();
        public void PriceTypesResetInput(object obj = null) => PriceTypeList = GetPriceTypes();
        
        private List<PriceType> GetPriceTypes()
        {
            LogService.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedRelationship == null) { Utilites.Error("Не выбран клиент"); return null; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Не выбран клиент"); return null; }
            var sql = SqlService.GET_PRICE_TYPES;
            if (string.IsNullOrEmpty(sql)) { Utilites.Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService<PriceType>.DocumentSelect(new List<string> { sql });
            return result;
        }

        private List<MatchingPriceTypes> GetMatchList()
        {
            LogService.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedRelationship == null) { Utilites.Error("Не выбран клиент"); return null; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Не выбран клиент"); return null; }
            var sql = SqlService.GET_MATCHED_PRICE_TYPES(SelectedRelationship.partnerIln);
            if (string.IsNullOrEmpty(sql)) { Utilites.Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService<MatchingPriceTypes>.DocumentSelect(new List<string> { sql });
            return result;
        }

    }
}
