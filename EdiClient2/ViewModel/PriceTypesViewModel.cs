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

namespace EdiClient.ViewModel
{
    public class PriceTypesViewModel : INotifyPropertyChanged
    {
        public PriceTypesViewModel(PriceTypesView page)
        {
            try
            {
                _page = page;
            } catch (Exception ex) { err( ex ); }
        }

        #region fields

        delegate void Message(string ex);
        Message msg = Utilites.Error;
        delegate void Error(Exception ex);
        Error err = Utilites.Error;
        
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
            _page.LoadDataButton.Content = "ждите загрузки";

           PriceTypeList = GetPriceTypes();
            MatchList = GetMatchList();

            _page.LoadDataButton.Content = "Загрузить данные";
            SetLayoutEnabled( true );

        }
        
        public void MakeMatching(object obj = null)
        {
            if (SelectedRelationship == null)
            {
                msg( "Не выбран покупатель" );
                return;
            }

            if (SelectedPriceType == null)
            {
                msg( "Не выбран тип цены" );
                return;
            }

            try
            {
                var sql = $"INSERT INTO ABT.REF_PRICE_TYPES_MATCHING(  CUSTOMER_GLN, ID_PRICE_TYPE, DISABLED)" +
                          $"VALUES('{SelectedRelationship.partnerIln}', { SelectedPriceType.ID}, 0)";
                DbService.Insert(sql);
                MatchList = GetMatchList();
            }
            catch (Exception ex) { err( ex ); }
        }
        
        
        public void DisposeMatching(object obj = null)
        {
            if (SelectedMatch == null)
            {
                msg( "Не выбран пункт с сопоставлением" );
                return;
            }
            
            try
            {
                DbService.Insert( $@"update abt.REF_Price_Types_MATCHING set DISABLED = 1
where CUSTOMER_GLN = '{SelectedMatch.CUSTOMER_GLN}' and ID_PRICE_TYPE = {SelectedMatch.ID_PRICE_TYPE}" );
                
                MatchList = GetMatchList();
            }
            catch (Exception ex) { Utilites.Error( ex ); }
        }
                        

        public void MatchSearch(object obj = null) =>
            Task.Factory.StartNew( () =>
            {
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
            => DbService<PriceType>.DocumentSelect(new List<string> { SqlService.GET_PRICE_TYPES });

        private List<MatchingPriceTypes> GetMatchList()
           => DbService<MatchingPriceTypes>.DocumentSelect(new List<string> { SqlService.GET_MATCHED_PRICE_TYPES(SelectedRelationship.partnerIln) });
        
    }
}
