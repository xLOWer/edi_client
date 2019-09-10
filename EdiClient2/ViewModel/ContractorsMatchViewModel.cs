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
using EdiClient.ViewModel;
using System.Windows.Data;
using static EdiClient.Services.Utils.Utilites;

namespace EdiClient.ViewModel
{
    public class ContractorsMatchViewModel : INotifyPropertyChanged
    {
        public ContractorsMatchViewModel()
        {
            //Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            LoadDataCommand.Execute(null);

        }

        protected void RaiseNotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #region props

        public event PropertyChangedEventHandler PropertyChanged;

        private Client selectedclient { get; set; }
        private List<Client> clientslist { get; set; }
        private List<Customer> customerslist { get; set; }
        private List<Customer> contractorslist { get; set; }
        private Customer linkeddeliverypointcontractor { get; set; }
        private Customer linkedbuyercustomer { get; set; }
        private Customer linkedsendercustomer { get; set; }

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
        
        public List<Customer> CustomersList
        {
            get { return customerslist; }
            set
            {
                customerslist = value;
                RaiseNotifyPropertyChanged("CustomersList");
            }
        }
        

        public List<Customer> ContractorsList
        {
            get { return contractorslist; }
            set
            {
                contractorslist = value;
                RaiseNotifyPropertyChanged("ContractorsList");
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

        public Command SelectedDeliveryPointContractorClearCommand => new Command(SelectedDeliveryPointContractorClear);
        public Command SelectedBuyerCustomerClearCommand => new Command(SelectedBuyerCustomerClear);
        public Command SelectedSenderCustomerClearCommand => new Command(SelectedSenderCustomerClear);        
        
        public Command LoadDataCommand => new Command(LoadData);
        public Command RemoveCommand => new Command((o) => { });
        public Command EditCommand => new Command((o) => {
            if (SelectedClient != null)
            {
                DeliveryAddress = SelectedClient?.DELIVERY_POINT_NAME ?? "";
                BuyerName = SelectedClient?.BUYER_NAME ?? "";

                SenderGln = SelectedClient?.SENDER_GLN ?? "";
                BuyerGln = SelectedClient?.BUYER_GLN ?? "";
                DeliveryGln = SelectedClient?.DELIVERY_POINT_GLN ?? "";

                LinkedSenderCustomer = CustomersList?.Where(x => x.Id == (SelectedClient?.SENDER_CUSTOMER_ID ?? "a"))?.FirstOrDefault() ?? null;
                LinkedBuyerCustomer = CustomersList?.Where(x => x.Id == (SelectedClient?.BUYER_CUSTOMER_ID ?? "a"))?.FirstOrDefault() ?? null;
                LinkedDeliveryPointContractor = ContractorsList?.Where(x => x.Id == (SelectedClient?.DELIVERY_POINT_CONTRACTOR_ID ?? "a")).FirstOrDefault() ?? null;
            }
        });

        public Command SaveCommand => new Command((o) => {
            //Logger.Log($"[CLIENT-MATCH] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");

            try
            {
                DbService.ExecuteCommand(new OracleCommand()
                {
                    Parameters =
                        {
                            new OracleParameter("P_SENDER_GLN", OracleDbType.NVarChar,SenderGln ?? "", ParameterDirection.Input),
                            new OracleParameter("P_BUYER_GLN", OracleDbType.NVarChar, BuyerGln ?? "", ParameterDirection.Input),
                            new OracleParameter("P_DELIVERY_POINT_GLN", OracleDbType.NVarChar, DeliveryGln ?? "", ParameterDirection.Input),
                            new OracleParameter("P_DELIVERY_POINT_NAME", OracleDbType.NVarChar, DeliveryAddress ?? "", ParameterDirection.Input),
                            new OracleParameter("P_BUYER_NAME", OracleDbType.NVarChar, BuyerName ?? "", ParameterDirection.Input),
                            new OracleParameter("P_SENDER_CUSTOMER_ID", OracleDbType.Number, LinkedSenderCustomer?.Id ?? "", ParameterDirection.Input),
                            new OracleParameter("P_BUYER_CUSTOMER_ID", OracleDbType.Number, LinkedBuyerCustomer?.Id ?? "", ParameterDirection.Input),
                            new OracleParameter("P_DELIVERY_POINT_CONTRACTOR_ID", OracleDbType.Number, LinkedDeliveryPointContractor?.Id ?? "", ParameterDirection.Input),
                        },
                    Connection = DbService.Connection.conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = (AppConfigHandler.conf.Schema + ".") + "EDI_MANAGER.CHANGE_DELIVERY_POINT"
                });
                LoadData();
            }
            catch (Exception ex) { Error(ex); }
        });
        public Command AddNewCommand => new Command((o) => {
            SelectedDeliveryPointContractorClear();
            SelectedBuyerCustomerClear();
            SelectedSenderCustomerClear();
            SelectedClient = null;
        });
        

        #endregion
        
        private void SelectedDeliveryPointContractorClear(object obj = null)
        {
            LinkedDeliveryPointContractor = null;
        }

        private void SelectedBuyerCustomerClear(object obj = null)
        {
            LinkedBuyerCustomer = null;
        }

        private void SelectedSenderCustomerClear(object obj = null)
        {
            LinkedSenderCustomer = null;
        }
         
        public void LoadData(object obj = null)
        {
            //Logger.Log($"[CLIENT-MATCH] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            ClientsList = GetList<Client>(DbService.Sqls.GET_CLIENTS);
            CustomersList = GetList<Customer>(DbService.Sqls.GET_CUSTOMERS);
            ContractorsList = GetList<Customer>(DbService.Sqls.GET_CONTRACTORS);
        }

        public List<T> GetList<T>(string sql)
        {
            //Logger.Log($"[GOODS] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (string.IsNullOrEmpty(sql)) { Error("Ошибка при выполнении загрузки"); return null; }
            var result = DbService.DocumentSelect<T>(new List<string> { sql }).Cast<T>().ToList();
            return result;
        }

    }
}
