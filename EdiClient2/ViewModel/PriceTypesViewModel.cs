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

        private List<Relation> relationshipList { get; set; }
        public List<Relation> RelationshipList
        {
            get { return relationshipList; }
            set
            {
                relationshipList = value;
                RaiseNotifyPropertyChanged("RelationshipList");
            }
        }

        private Relation selectedRelationship { get; set; }
        public Relation SelectedRelationship
        {
            get { return selectedRelationship; }
            set
            {
                selectedRelationship = value;
                RaiseNotifyPropertyChanged( "SelectedRelationship" );
            }
        }

        private PriceTypesView _page { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isNewMatchingEnabled = true;
        public virtual bool IsNewMatchingEnabled
        {
            get
            {
                return !String.IsNullOrEmpty( NewCustomerItemCode );/* || (NewCustomerItemCode?.Length ?? 0) >= 4 ) && SelectedPriceTypes != null*/;
            }
            set
            {
                isNewMatchingEnabled = value;
                RaiseNotifyPropertyChanged( "IsNewMatchingEnabled" );
            }
        }


        private PriceType selectedPriceTypes = new PriceType();
        public PriceType SelectedPriceTypes
        {
            get { return selectedPriceTypes; }
            set
            {
                selectedPriceTypes = value;
                RaiseNotifyPropertyChanged( "SelectedPriceTypes" );
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

        private string relationshipSearchText = "";
        public string RelationshipSearchText
        {
            get { return relationshipSearchText; }
            set
            {
                relationshipSearchText = value;
                RaiseNotifyPropertyChanged( "RelationshipSearchText" );
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
        
        private List<MatchingPriceTypes> matchesList = new List<MatchingPriceTypes>();
        public List<MatchingPriceTypes> MatchesList
        {
            get { return matchesList; }
            set
            {
                matchesList = value;
                RaiseNotifyPropertyChanged( "MatchesList" );
            }
        }

        private bool HelpMode { get; set; } = false;
        
        public CommandService RelationshipSearchCommand => new CommandService( RelationshipSearch );
        public CommandService MatchesSearchCommand => new CommandService( MatchesSearch );
        public CommandService PriceTypeSearchCommand => new CommandService( PriceTypeSearch );
        public CommandService LoadDataCommand => new CommandService( LoadData );
        public CommandService MakeMatchingCommand => new CommandService( MakeMatching );
        public CommandService DisposeMatchingCommand => new CommandService( DisposeMatching );
        public CommandService RelationshipResetInputCommand => new CommandService( RelationshipResetInput );
        public CommandService MatchesResetInputCommand => new CommandService( MatchesResetInput );
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
            RelationshipList = GetRelationships();
            MatchesList = GetMatchesList();

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

            if (SelectedPriceTypes == null)
            {
                msg( "Не выбран тип цены" );
                return;
            }

            try
            {
                var sql = $"insert into abt.REF_PRICE_TYPES_MATCHING(CUSTOMER_GLN, CUSTOMER_ARTICLE, ID_PriceTypes, DISABLED)" +
                $"values(4650093209994, '{SelectedRelationship.partnerIln}', {SelectedPriceTypes.PriceId}, 0)";
                DbService.Insert(sql);            
                RelationshipList = GetRelationships();
                MatchesList = GetMatchesList();
            }
            catch (Exception ex) { err( ex ); }
        }


        
        public void DisposeMatching(object obj = null)
        {
            /*if (SelectedMatch == null)
            {
                msg( "Не выбран пункт с сопоставлением" );
            }

            if (String.IsNullOrEmpty( SelectedMatch.CustomerPriceTypesId ))
            {
                msg( "У выбранного товара отсутствует код покупателя" );
            }

            try
            {
                DbService.Insert( $@"update abt.REF_PriceType_MATCHING set DISABLED=1
where CUSTOMER_GLN = 4650093209994 and CUSTOMER_ARTICLE = '{SelectedRelationship.BuyerItemCode}'" );

                RelationshipList = GetRelationships();
                RelationshipResetInput();
                MatchesList = GetMatchesList();
            }
            catch (Exception ex) { Utilites.Error( ex ); }*/

        }



        public void RelationshipResetInput(object obj = null)
        {
            RelationshipSearch( "" );
        }

        public void MatchesResetInput(object obj = null)
        {
            MatchesSearch( "" );
        }

        public void PriceTypesResetInput(object obj = null)
        {
            PriceTypeSearch( "" );
        }

        public void RelationshipSearch(object obj = null)
        {
            Task.Factory.StartNew( () =>
            {
                RelationshipList = GetRelationships();
                if (!String.IsNullOrEmpty( RelationshipSearchText ))
                {
                    var searchList = RelationshipSearchText.Split( ' ' );
                    if (searchList.Count() > 0)
                    {
                        foreach (var item in searchList)
                        {
                            var text = item?.ToUpper()?.Trim( ' ' ) ?? "";
                            if (!String.IsNullOrEmpty( item ))
                            {
                                RelationshipList = RelationshipList.Where(x => x.Name?.ToUpper()?.Contains( text ) ?? false).ToList();
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
                    var searchList = MatchesSearchText.Split( ' ' );
                    if (searchList.Count() > 0)
                    {
                        foreach (var item in searchList)
                        {
                            if (!String.IsNullOrEmpty( item ))
                            {
                                var text = item?.ToUpper()?.Trim( ' ' ) ?? "";
                                MatchesList = MatchesList.Where(
                                    x => (x.CUSTOMER_GLN?.ToUpper()?.Contains( text ) ?? false)
                                      || (x.ID_PRICE_TYPE?.ToUpper()?.Contains(text) ?? false)
                                      || (x.CUSTOMER_GLN?.ToUpper()?.Contains(text) ?? false)
                                      || (x.CUSTOMER_GLN?.ToUpper()?.Contains(text) ?? false)
                                ).ToList();
                            }
                        }
                    }
                }
            } );
        }
        
        public void PriceTypeSearch(object obj = null)
        {
            Task.Factory.StartNew( () =>
            {
                PriceTypeList = GetPriceTypes();
                if (!String.IsNullOrEmpty( PriceTypeSearchText ))
                {
                    var searchList = PriceTypeSearchText.Split( ' ' );
                    if (searchList.Count() > 0)
                    {
                        foreach (var item in searchList)
                        {
                            if (!String.IsNullOrEmpty( item ))
                            {
                                var text = item?.ToUpper()?.Trim( ' ' ) ?? "";
                                PriceTypeList = PriceTypeList.Where(
                                    x => (x.PriceName?.ToUpper().Trim( ' ' ).Contains( text ) ?? false)
                                      || (x.PriceId?.ToUpper().Trim( ' ' ).Contains( text ) ?? false)
                                ).ToList();
                            }
                        }
                    }
                }
            } );
        }
        


        private List<PriceType> GetPriceTypes()
        {
            var list = new List<string>
            {
                SqlConfiguratorService.Sql_SelectPriceTypes()
            };
            return DbService<PriceType>.DocumentSelect( list );
        }
        
        private List<Relation> GetRelationships()        
            => EdiService.Relationships().Where(x => x.documentType == "ORDER").ToList() ?? throw new Exception("При загрузке списка покупателей");
        
        
        private List<MatchingPriceTypes> GetMatchesList()
        {
            var list = new List<string>
            {
                SqlConfiguratorService.Sql_SelectMatches()
            };
            return DbService<Matches>.DocumentSelect( list );
        }


        

    }
}
