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
using EdiClient.AppSettings;
using System.Reflection;
using EdiClient.ViewModel.Common;
using System.Windows.Data;

namespace EdiClient.ViewModel
{
    public class ContractorsMatchViewModel : INotifyPropertyChanged
    {
        public ContractorsMatchViewModel()
        {

        }

        protected void RaiseNotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private string clientsearchtext { get; set; }
        private string buyercustomersearchtext { get; set; }
        private string sendercustomersearchtext { get; set; }
        private string deliverypointcontractorsearchtext { get; set; }
        private Client selectedclient { get; set; }
        private List<Client> clientslist { get; set; }
        private List<Customer> sendercustomerslist { get; set; }
        private List<Customer> buyercustomerslist { get; set; }
        private List<Customer> deliverypointcontractorslist { get; set; }
        private ListCollectionView clientsgrouplist { get; set; }
        private Customer linkeddeliverypointcontractor { get; set; }
        private Customer linkedbuyercustomer { get; set; }
        private Customer linkedsendercustomer { get; set; }
        private Customer selectedbuyercustomer { get; set; }
        private Customer selectedsendercustomer { get; set; }
        private Customer selecteddeliverypointcontractor { get; set; }


        public string ClientSearchText
        {
            get { return clientsearchtext; }
            set
            {
                clientsearchtext = value;
                RaiseNotifyPropertyChanged("ClientSearchText");
            }
        }

        public string BuyerCustomerSearchText
        {
            get { return buyercustomersearchtext; }
            set
            {
                buyercustomersearchtext = value;
                RaiseNotifyPropertyChanged("BuyerCustomerSearchText");
            }
        }

        public string SenderCustomerSearchText
        {
            get { return sendercustomersearchtext; }
            set
            {
                sendercustomersearchtext = value;
                RaiseNotifyPropertyChanged("SenderCustomerSearchText");
            }
        }

        public string DeliveryPointContractorSearchText
        {
            get { return deliverypointcontractorsearchtext; }
            set
            {
                deliverypointcontractorsearchtext = value;
                RaiseNotifyPropertyChanged("DeliveryPointContractorSearchText");
            }
        }

        public Customer SelectedBuyerCustomer
        {
            get { return selectedbuyercustomer; }
            set
            {
                selectedbuyercustomer = value;
                RaiseNotifyPropertyChanged("SelectedBuyerCustomer");
            }
        }

        public Customer SelectedSenderCustomer
        {
            get { return selectedsendercustomer; }
            set
            {
                selectedsendercustomer = value;
                RaiseNotifyPropertyChanged("SelectedSenderCustomer");
            }
        }

        public Customer SelectedDeliveryPointContractor
        {
            get { return selecteddeliverypointcontractor; }
            set
            {
                selecteddeliverypointcontractor = value;
                RaiseNotifyPropertyChanged("SelectedDeliveryPointContractor");
            }
        }

        public Client SelectedClient
        {
            get { return selectedclient; }
            set
            {
                selectedclient = value;
                RaiseNotifyPropertyChanged("SelectedClient");
            }
        }

        public List<Client> ClientsList
        {
            get { return clientslist; }
            set
            {
                clientslist = value;
                RaiseNotifyPropertyChanged("ClientsList");
            }
        }

        public ListCollectionView ClientsGroupList
        {
            get { return clientsgrouplist; }
            set
            {
                clientsgrouplist = value;
                RaiseNotifyPropertyChanged("ClientsGroupList");
            }
        }

        public List<Customer> SenderCustomersList
        {
            get { return sendercustomerslist; }
            set
            {
                sendercustomerslist = value;
                RaiseNotifyPropertyChanged("SenderCustomersList");
            }
        }

        public List<Customer> BuyerCustomersList
        {
            get { return buyercustomerslist; }
            set
            {
                buyercustomerslist = value;
                RaiseNotifyPropertyChanged("BuyerCustomersList");
            }
        }
        
        public List<Customer> DeliveryPointContractorsList
        {
            get { return deliverypointcontractorslist; }
            set
            {
                deliverypointcontractorslist = value;
                RaiseNotifyPropertyChanged("DeliveryPointContractorsList");
            }
        }

        public Customer LinkedDeliveryPointContractor
        {
            get { return linkeddeliverypointcontractor; }
            set
            {
                linkeddeliverypointcontractor = value;
                RaiseNotifyPropertyChanged("LinkedDeliveryPointContractor");
            }
        }
        
        public Customer LinkedBuyerCustomer
        {
            get { return linkedbuyercustomer; }
            set
            {
                linkedbuyercustomer = value;
                RaiseNotifyPropertyChanged("LinkedBuyerCustomer");
            }
        }

        public Customer LinkedSenderCustomer
        {
            get { return linkedsendercustomer; }
            set
            {
                linkedsendercustomer = value;
                RaiseNotifyPropertyChanged("LinkedSenderCustomer");
            }
        }
        
        public CommandService DeliveryPointContractorSearchCommand => new CommandService(DeliveryPointContractorSearch);
        public CommandService DeliveryPointContractorResetInputCommand => new CommandService(DeliveryPointContractorResetInput);
        public CommandService BuyerCustomerSearchCommand => new CommandService(BuyerCustomerSearch);
        public CommandService BuyerCustomerResetInputCommand => new CommandService(BuyerCustomerResetInput);
        public CommandService SenderCustomerSearchCommand => new CommandService(SenderCustomerSearch);
        public CommandService SenderCustomerResetInputCommand => new CommandService(SenderCustomerResetInput);
        public CommandService ClientSearchCommand => new CommandService(ClientSearch);
        public CommandService ClientResetInputCommand => new CommandService(ClientResetInput);

        public CommandService CommandServiceLoadDataCommand => new CommandService(CommandServiceLoadData);
        public CommandService RemoveCommand => new CommandService(Remove);
        public CommandService EditCommand => new CommandService(Edit);
        public CommandService LoadDataCommand => new CommandService(LoadData);

        public void DeliveryPointContractorSearch(object obj = null)
        {
        }

        public void DeliveryPointContractorResetInput(object obj = null)
        {
        }

        public void BuyerCustomerSearch(object obj = null)
        {
        }

        public void BuyerCustomerResetInput(object obj = null)
        {
        }

        public void SenderCustomerSearch(object obj = null)
        {
        }

        public void SenderCustomerResetInput(object obj = null)
        {
        }

        public void ClientSearch(object obj = null)
        {
        }

        public void ClientResetInput(object obj = null)
        {
        }

        public void CommandServiceLoadData(object obj = null)
        {
        }

        public void Remove(object obj = null)
        {
        }

        public void Edit(object obj = null)
        {
            if((SenderCustomersList?.Count ?? 1) > 0) SenderCustomersList = DocumentRepository.GetList<Customer>(SqlService.GET_CUSTOMERS);             
            if ((DeliveryPointContractorsList?.Count ?? 1) > 0) DeliveryPointContractorsList = DocumentRepository.GetList<Customer>(SqlService.GET_CONTRACTORS);
            BuyerCustomersList = SenderCustomersList;

            if (!string.IsNullOrEmpty(SelectedClient.SENDER_CUSTOMER_ID))
                SelectedSenderCustomer = SenderCustomersList.First(x=>x.Id == SelectedClient.SENDER_CUSTOMER_ID);

            if (!string.IsNullOrEmpty(SelectedClient.BUYER_CUSTOMER_ID))
                SelectedBuyerCustomer = BuyerCustomersList.First(x => x.Id == SelectedClient.BUYER_CUSTOMER_ID);

            if (!string.IsNullOrEmpty(SelectedClient.DELIVERY_POINT_CONTRACTOR_ID))
                SelectedDeliveryPointContractor = DeliveryPointContractorsList.First(x => x.Id == SelectedClient.DELIVERY_POINT_CONTRACTOR_ID);
        }
        

        public void LoadData(object obj = null)
        {
            LogService.Log($"[GOODS] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            ClientsList = DocumentRepository.GetList<Client>(SqlService.GET_CLIENTS);
            ClientsGroupList = new ListCollectionView(ClientsList);
            ClientsGroupList.GroupDescriptions.Add(new PropertyGroupDescription("SENDER_GLN"));
            ClientsGroupList.GroupDescriptions.Add(new PropertyGroupDescription("BUYER_GLN"));

        }

    }
}
