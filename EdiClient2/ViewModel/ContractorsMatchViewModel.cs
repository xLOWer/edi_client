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

        #region props

        public event PropertyChangedEventHandler PropertyChanged;

        private Client selectedclient { get; set; }
        private List<Client> clientslist { get; set; }
        private List<Customer> sendercustomerslist { get; set; }
        private List<Customer> buyercustomerslist { get; set; }
        private List<Customer> deliverypointcontractorslist { get; set; }
        private Customer linkeddeliverypointcontractor { get; set; }
        private Customer linkedbuyercustomer { get; set; }
        private Customer linkedsendercustomer { get; set; }
        private Customer selectedbuyercustomer { get; set; }
        private Customer selectedsendercustomer { get; set; }
        private Customer selecteddeliverypointcontractor { get; set; }

        private string sendergln { get; set; }
        private string buyergln { get; set; }
        private string deliverygln { get; set; }

        private string deliveryaddress { get; set; }
        private string buyername { get; set; }

        public string DeliveryAddress
        {
            get { return deliveryaddress; }
            set
            {
                deliveryaddress = value;
                RaiseNotifyPropertyChanged("DeliveryAddress");
            }
        }

        public string BuyerName
        {
            get { return buyername; }
            set
            {
                buyername = value;
                RaiseNotifyPropertyChanged("BuyerName");
            }
        }

        public string SenderGln
        {
            get { return sendergln; }
            set
            {
                sendergln = value;
                RaiseNotifyPropertyChanged("SenderGln");
            }
        }
        public string BuyerGln
        {
            get { return buyergln; }
            set
            {
                buyergln = value;
                RaiseNotifyPropertyChanged("BuyerGln");
            }
        }
        public string DeliveryGln
        {
            get { return deliverygln; }
            set
            {
                deliverygln = value;
                RaiseNotifyPropertyChanged("DeliveryGln");
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

        public CommandService SelectedDeliveryPointContractorClearCommand => new CommandService(SelectedDeliveryPointContractorClear);
        public CommandService SelectedBuyerCustomerClearCommand => new CommandService(SelectedBuyerCustomerClear);
        public CommandService SelectedSenderCustomerClearCommand => new CommandService(SelectedSenderCustomerClear);        
        
        public CommandService LoadDataCommand => new CommandService(LoadData);
        public CommandService RemoveCommand => new CommandService(Remove);
        public CommandService EditCommand => new CommandService(Edit);
        public CommandService SaveCommand => new CommandService(Save);
        public CommandService AddNewCommand => new CommandService(AddNew);

        #endregion

        private void AddNew(object obj = null)
        {
            SelectedDeliveryPointContractorClear();
            SelectedBuyerCustomerClear();
            SelectedSenderCustomerClear();
            SelectedClient = null;
        }

        private void SelectedDeliveryPointContractorClear(object obj = null)
        {
            LinkedDeliveryPointContractor = null;
            SelectedDeliveryPointContractor = null;
        }

        private void SelectedBuyerCustomerClear(object obj = null)
        {
            LinkedBuyerCustomer = null;
            SelectedBuyerCustomer = null;
        }

        private void SelectedSenderCustomerClear(object obj = null)
        {
            LinkedSenderCustomer = null;
            SelectedSenderCustomer = null;
        }
         
        public void Save(object obj = null)
        {
            LogService.Log($"[GOODS] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");

            double eval1 = double.Parse(SenderGln);

            LinkedSenderCustomer = SelectedSenderCustomer;
            LinkedBuyerCustomer = SelectedBuyerCustomer;
            LinkedDeliveryPointContractor = SelectedDeliveryPointContractor;

            try
            {
                DbService.ExecuteCommand(new OracleCommand()
                {
                    Parameters =
                        {
                            new OracleParameter("P_SENDER_GLN", OracleDbType.NVarChar,SenderGln, ParameterDirection.Input),
                            new OracleParameter("P_BUYER_GLN", OracleDbType.NVarChar, BuyerGln, ParameterDirection.Input),
                            new OracleParameter("P_DELIVERY_POINT_GLN", OracleDbType.NVarChar, DeliveryGln, ParameterDirection.Input),
                            new OracleParameter("P_DELIVERY_POINT_NAME", OracleDbType.NVarChar, DeliveryAddress, ParameterDirection.Input),
                            new OracleParameter("P_BUYER_NAME", OracleDbType.NVarChar, BuyerName, ParameterDirection.Input),
                            new OracleParameter("P_SENDER_CUSTOMER_ID", OracleDbType.Number, LinkedSenderCustomer.Id, ParameterDirection.Input),
                            new OracleParameter("P_BUYER_CUSTOMER_ID", OracleDbType.Number, LinkedBuyerCustomer.Id, ParameterDirection.Input),
                            new OracleParameter("P_DELIVERY_POINT_CONTRACTOR_ID", OracleDbType.Number, LinkedDeliveryPointContractor.Id, ParameterDirection.Input),
                        },
                    Connection = OracleConnectionService.conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = (AppConfig.Schema + ".") + "EDI_CHANGE_DELIVERY_POINT"
                });
                LoadData();
            }
            catch (Exception ex) { Utilites.Error(ex); }
        }

        public void Remove(object obj = null)              { }

        public void Edit(object obj = null)
        {
            if(SelectedClient != null)
            {
                DeliveryAddress = SelectedClient.DELIVERY_POINT_NAME;
                BuyerName = SelectedClient.BUYER_NAME;

                if (!string.IsNullOrEmpty(SelectedClient?.SENDER_CUSTOMER_ID))
                {
                    SenderGln = SelectedClient.SENDER_GLN;
                    LinkedSenderCustomer = SenderCustomersList?.First(x => x.Id == SelectedClient?.SENDER_CUSTOMER_ID) ?? null;
                    SelectedSenderCustomer = LinkedSenderCustomer;
                }
                else { SelectedSenderCustomer = null; LinkedSenderCustomer = null; }

                if (!string.IsNullOrEmpty(SelectedClient?.BUYER_CUSTOMER_ID))
                {
                    BuyerGln = SelectedClient.BUYER_GLN;
                    LinkedBuyerCustomer = SenderCustomersList?.First(x => x.Id == SelectedClient?.SENDER_CUSTOMER_ID) ?? null;
                    SelectedBuyerCustomer = LinkedBuyerCustomer;
                }
                else { SelectedBuyerCustomer = null; LinkedBuyerCustomer = null; }

                if (!string.IsNullOrEmpty(SelectedClient?.DELIVERY_POINT_CONTRACTOR_ID))
                {
                    DeliveryGln = SelectedClient.DELIVERY_POINT_GLN;
                    LinkedDeliveryPointContractor = SenderCustomersList?.First(x => x.Id == SelectedClient?.SENDER_CUSTOMER_ID) ?? null;
                    SelectedDeliveryPointContractor = LinkedDeliveryPointContractor;
                }
                else { SelectedDeliveryPointContractor = null; LinkedDeliveryPointContractor = null; }
            }
        }

        public void LoadData(object obj = null)
        {
            LogService.Log($"[GOODS] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            GetClients();
            GetBuyers();
            GetDelivery();
        }

        private void GetSenders() => SenderCustomersList = DocumentRepository.GetList<Customer>(SqlService.GET_CUSTOMERS);
        private void GetDelivery() => DeliveryPointContractorsList = DocumentRepository.GetList<Customer>(SqlService.GET_CONTRACTORS);
        private void GetBuyers()
        {
            GetSenders();
            BuyerCustomersList = SenderCustomersList;
        }
        private void GetClients()
        {
            ClientsList = DocumentRepository.GetList<Client>(SqlService.GET_CLIENTS);
        }
        

    }
}
