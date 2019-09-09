﻿using Devart.Data.Oracle;
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
            Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
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
        
        public CommandService LoadDataCommand => new CommandService( LoadData );
        public CommandService MakeMatchingCommand => new CommandService( MakeMatching );
        public CommandService DisposeMatchingCommand => new CommandService( DisposeMatching );

        #endregion

        protected void RaiseNotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( info ) );
        }            
        
        
        public void LoadData(object obj = null)
        {
           PriceTypeList = GetPriceTypes();
           MatchList = GetMatchList();    
        }
        
        public void MakeMatching(object obj = null)
        {
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
                    CommandText = (AppConfigHandler.conf.Schema + ".") + "EDI_MANAGER.MAKE_PRICE_LINK"
                });

                MatchList = GetMatchList();
            }
            catch (Exception ex) { Error( ex ); }
        }
        
        
        public void DisposeMatching(object obj = null)
        {
            Logger.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedMatch == null) { Error( "Не выбран пункт с сопоставлением" ); return; }
            
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
                MatchList = GetMatchList();
            }
            catch (Exception ex) { Error( ex ); }
        }
                  
        private List<PriceType> GetPriceTypes()
        {
            Logger.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedRelationship == null) { Error("Не выбран клиент"); return null; }
            if (SelectedRelationship.partnerIln == null) { Error("Не выбран клиент"); return null; }
            var sql = DbService.Sqls.GET_PRICE_TYPES;
            if (string.IsNullOrEmpty(sql)) { Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService.DocumentSelect<PriceType>(new List<string> { sql });
            return result;
        }

        private List<MatchingPriceTypes> GetMatchList()
        {
            Logger.Log($"[PRICE] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedRelationship == null) { Error("Не выбран клиент"); return null; }
            if (SelectedRelationship.partnerIln == null) { Error("Не выбран клиент"); return null; }
            var sql = DbService.Sqls.GET_MATCHED_PRICE_TYPES(SelectedRelationship.partnerIln);
            if (string.IsNullOrEmpty(sql)) { Error("Ошибка при выполнении загрузки списка сопоставленных товаров"); return null; }
            var result = DbService.DocumentSelect<MatchingPriceTypes>(new List<string> { sql });
            return result;
        }

    }
}
