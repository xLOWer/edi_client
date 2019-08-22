using Devart.Data.Oracle;
using EdiClient.Model.MatchingDbModel;
using EdiClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using EdiClient.View;
using static EdiClient.Model.WebModel.RelationResponse;
using EdiClient.AppSettings;
using System.Reflection;

namespace EdiClient.ViewModel
{
    public class PriceTypesViewModel : INotifyPropertyChanged
    {
        public PriceTypesViewModel(PriceTypesView page)
        {
            EdiClient.Services.Utilites.Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
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
        
        public CommandService LoadDataCommand => new CommandService( LoadData );
        public CommandService MakeMatchingCommand => new CommandService( MakeMatching );
        public CommandService DisposeMatchingCommand => new CommandService( DisposeMatching );

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
            Utilites.Logger.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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
                    Connection = DbService.Connection.conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = (AppConfig.Schema + ".") + "EDI_MAKE_PRICE_LINK"
                });

                MatchList = GetMatchList();
            }
            catch (Exception ex) { Utilites.Error( ex ); }
        }
        
        
        public void DisposeMatching(object obj = null)
        {
            Utilites.Logger.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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
                    Connection = DbService.Connection.conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = (AppConfig.Schema + ".") + "EDI_MAKE_PRICE_UNLINK"
                });
                MatchList = GetMatchList();
            }
            catch (Exception ex) { Utilites.Error( ex ); }
        }
                  
        private List<PriceType> GetPriceTypes()
        {
            Utilites.Logger.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedRelationship == null) { Utilites.Error("Не выбран клиент"); return null; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Не выбран клиент"); return null; }
            var sql = DbService.Sqls.GET_PRICE_TYPES;
            if (string.IsNullOrEmpty(sql)) { Utilites.Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService.DocumentSelect<PriceType>(new List<string> { sql });
            return result;
        }

        private List<MatchingPriceTypes> GetMatchList()
        {
            Utilites.Logger.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedRelationship == null) { Utilites.Error("Не выбран клиент"); return null; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Не выбран клиент"); return null; }
            var sql = DbService.Sqls.GET_MATCHED_PRICE_TYPES(SelectedRelationship.partnerIln);
            if (string.IsNullOrEmpty(sql)) { Utilites.Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService.DocumentSelect<MatchingPriceTypes>(new List<string> { sql });
            return result;
        }

    }
}
