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
using static EdiClient.Services.Utils.Utilites;

namespace EdiClient.ViewModel
{
    public class PriceTypesViewModel : INotifyPropertyChanged
    {
        public PriceTypesViewModel()
        {
            LoadDataCommand.Execute(null);
            Logger.Log($"[PriceTypesViewModel]PRICES");
        }

        #region fields
                
        public List<Relation> Relationships => EdiService.Relationships;
        public Relation SelectedRelationship => EdiService.SelectedRelationship;
        public int RelationshipCount => EdiService.RelationshipCount;
        
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
        
        public Command LoadDataCommand => new Command( (o) =>{
            GetPriceTypes();
            GetMatchList();
            Logger.Log($"[LoadDataCommand]PRICES|PriceTypeListCount={PriceTypeList.Count}|MatchListCount={MatchList.Count}");
        });
        public Command MakeMatchingCommand => new Command( (o) =>
        {
            Logger.Log($"[MakeMatchingCommand]PRICES|P_CUSTOMER_GLN={SelectedMatch.CUSTOMER_GLN}|ID_PRICE_TYPE={SelectedMatch.ID_PRICE_TYPE}");
            try
            {
                DbService.ExecuteCommand(new OracleCommand()
                {
                    Parameters =
                        {
                            new OracleParameter("P_CUSTOMER_GLN", OracleDbType.Number,SelectedMatch.CUSTOMER_GLN, ParameterDirection.Input),
                            new OracleParameter("ID_PRICE_TYPE", OracleDbType.NVarChar, SelectedMatch.ID_PRICE_TYPE, ParameterDirection.Input),
                        },
                    CommandType = CommandType.StoredProcedure,
                    CommandText = (AppConfigHandler.conf.Schema + ".") + "EDI_MANAGER.MAKE_PRICE_LINK"
                });

                GetMatchList();
            }
            catch (Exception ex) { Error(ex); }

        });
        public Command DisposeMatchingCommand => new Command( (o) =>
        {
            if (SelectedMatch == null) { Error("Не выбран пункт с сопоставлением"); return; }
            Logger.Log($"[DisposeMatchingCommand]PRICES|P_CUSTOMER_GLN={SelectedMatch.CUSTOMER_GLN}|ID_PRICE_TYPE={SelectedMatch.ID_PRICE_TYPE}");

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
                    CommandText = (AppConfigHandler.conf.Schema + ".") + "EDI_MANAGER.MAKE_PRICE_UNLINK"
                });
                GetMatchList();
            }
            catch (Exception ex) { Error(ex); }
        });

        #endregion

        protected void RaiseNotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( info ) );
        }            
                
                  
        private void GetPriceTypes()
        {
            if (SelectedRelationship == null) { Error("Не выбран клиент"); return; }
            if (SelectedRelationship.partnerIln == null) { Error("Не выбран клиент"); return; }
            var sql = DbService.Sqls.GET_PRICE_TYPES;
            if (string.IsNullOrEmpty(sql)) { Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return; }
            PriceTypeList = DbService.DocumentSelect<PriceType>(new List<string> { sql });            
        }

        private void GetMatchList()
        {
            if (SelectedRelationship == null) { Error("Не выбран клиент"); return; }
            if (SelectedRelationship.partnerIln == null) { Error("Не выбран клиент"); return; }
            var sql = DbService.Sqls.GET_MATCHED_PRICE_TYPES(SelectedRelationship.partnerIln);
            if (string.IsNullOrEmpty(sql)) { Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return; }
            MatchList = DbService.DocumentSelect<MatchingPriceTypes>(new List<string> { sql });
        }

    }
}
