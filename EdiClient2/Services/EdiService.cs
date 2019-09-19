using EdiClient.Model.WebModel;
using Edisoft.WebService.EdiWebService;
using System;
using System.IO;
using System.Windows;
using System.ServiceModel;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.ServiceModel.Description;
using static EdiClient.Model.WebModel.RelationResponse;
using System.Reflection;
using System.Linq;
using EdiClient.AppSettings;
using System.Net;
using static EdiClient.Services.Utils.Utilites;

namespace EdiClient.Services
{
    internal static class EdiService
    {
        internal static EDIWebServicePortTypeClient Client { get; set; }
        internal static ServiceEndpoint endPoint;
        internal static EndpointAddress address;

        public static List<Relation> Relationships { get; set; }

        private static Relation selectedRelationship;

        public static Relation SelectedRelationship
        {
            get
            {
                if (Relationships == null || selectedRelationship == null)
                    return null;
                else
                    return selectedRelationship ?? Relationships[0] ?? new Relation();
            }
            set { selectedRelationship = value; }
        }

        public static int RelationshipCount { get; set; } = 0;

        internal static void UpdateData()
        {
            if (!string.IsNullOrEmpty(AppConfigHandler.conf.EdiPassword) && !string.IsNullOrEmpty(AppConfigHandler.conf.EdiGLN) && !string.IsNullOrEmpty(AppConfigHandler.conf.EdiUser))
            {
                var rl = GetRelationships() ?? new List<Relation>();
                if (rl.Count == 0) { return; }
                var rels = rl.Where(x => x.documentType == "ORDER").ToList(); ;
                Relationships = rels;
                SelectedRelationship = SelectedRelationship ?? (Relationships[0]);
                RelationshipCount = Relationships.Count;
            }
        }

        internal static EDIWebServicePortTypeClient Configure(EndpointAddress _address = null)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            Client = new EDIWebServicePortTypeClient();
            endPoint = Client.Endpoint;
            if (_address != null)
            {
                address = _address;
                Client.Endpoint.Address = address;
            }


            if (AppConfigHandler.conf.EnableProxy == true)
            {
                // задать настройки прокси как у системы
                GlobalProxySelection.Select = WebProxy.GetDefaultProxy();

                if (!String.IsNullOrEmpty(AppConfigHandler.conf.ProxyUserName) && !String.IsNullOrEmpty(AppConfigHandler.conf.ProxyUserPassword))
                    // задаём данные для аутентификации пользователя прокси
                    GlobalProxySelection.Select.Credentials = new NetworkCredential(AppConfigHandler.conf.ProxyUserName, AppConfigHandler.conf.ProxyUserPassword);
                else Error("Не удалось задать пользователя прокси.\nДанные пользователя не верны");
            }
            else
            {
                // сбрасываем данные для аутентификации пользователя прокси
                GlobalProxySelection.Select.Credentials = null;
            }
            
            return Client;
        }

        internal static EDIWebServicePortTypeClient Configure(string _endPointConfigurationName, EndpointAddress _endpointAddress)
        {
            Client = new EDIWebServicePortTypeClient(_endPointConfigurationName, _endpointAddress);
            return Client;
        }

        internal static string ResponseErrorHandler(retRes res)
        {
            if (res == null) return "";
            var result = "";
            switch (res.res)
            {
                case "00000001": result = "Ошибка аутентификации."; break;
                case "00000002": result = "Ошибка во взаимосвязи."; break;
                case "00000003": result = "Внешняя ошибка."; break;
                case "00000004": result = "Внутренняя ошибка сервера."; break;
                case "00000005": result = "Превышен таймаут на выполнение метода."; break;
                case "00000006": result = "Ошибка Web."; break;
                case "00000007": result = "Некорректные параметры."; break;
                case "00000008": result = "В доступе на платформу отказано."; break;
                case "00000009": result = "Запрашиваемого документа не обнаружено."; break;
            }
            return result;
        }


        internal static OrganizationInfo OrganizationInfo(string inn, string kpp, string ogrn, string fnsId, string gln)
        {
            //Logger.Log($"[EDI] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            retRes returnedResult = null;
            XmlSerializer serializer = new XmlSerializer(typeof(OrganizationInfo));

            returnedResult = Client.organizationInfo(AppConfigHandler.conf.EdiUser, AppConfigHandler.conf.EdiPassword, inn, kpp, ogrn, fnsId, gln);

            if (returnedResult == null) Error("Нет соединения с edisoft");

            if (returnedResult.res == "00000000")
                return (OrganizationInfo)serializer.Deserialize(new StringReader(returnedResult.cnt));
            else
                MessageBox.Show(ResponseErrorHandler(returnedResult));

            return null;
        }

        internal static List<Relation> GetRelationships(int timeout = 5000)
        {
            //Logger.Log($"[EDI] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            var ser = new XmlSerializer(typeof(RelationResponse));
            retRes returnedResult = null;
            try
            {
                returnedResult = Client?.relationships(AppConfigHandler.conf.EdiUser, AppConfigHandler.conf.EdiPassword, timeout);
            }
            catch (Exception ex) { Error(ex); }
            
            if (returnedResult == null) Error("Запрос связей не дал результатов");
            
            if (returnedResult?.res == "00000000")
            {
                var reader = new StringReader(returnedResult.cnt);
                return ((RelationResponse)ser.Deserialize(reader)).Relations;
            }
            else
            {
                MessageBox.Show(ResponseErrorHandler(returnedResult));
            }
            
            return null;
        }


        internal static List<TModel> Receive<TModel>(string partnerILN, string documentType, string trackingId, string documentStandard, string changeDocumentStatus, int timeout = 5000)
        {
            //Logger.Log($"[EDI] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            var ser = new XmlSerializer(typeof(TModel));
            retRes returnedResult = null;

            returnedResult = Client?.receive(AppConfigHandler.conf.EdiUser, AppConfigHandler.conf.EdiPassword, partnerILN, documentType, trackingId, documentStandard, changeDocumentStatus, timeout);

            if (returnedResult == null) Error("Нет соединения с edisoft");

            if (returnedResult?.res == "00000000")
            {
                var xml = XmlService<TModel>.Deserialize(returnedResult.cnt);
                return xml;
            }
            else
                MessageBox.Show(ResponseErrorHandler(returnedResult));

            return null;
        }


        internal static void Send(string partnerILN, string documentType, string documentVersion, string documentStandard, string documentTest, string controlNumber, string documentContent, int timeout = 5000)
        {
            //Logger.Log($"[EDI] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            retRes returnedResult = null;

            returnedResult = Client.send(AppConfigHandler.conf.EdiUser, AppConfigHandler.conf.EdiPassword, partnerILN, documentType, documentVersion, documentStandard, documentTest, controlNumber, documentContent, timeout);

            if (returnedResult?.res != "00000000" && returnedResult != null)
            {
                MessageBox.Show("КОД " + returnedResult.res + "\n\n" + returnedResult.cnt);
            }
        }

        internal static List<DocumentInfo> ListMBAll(bool getBinaryData = false)
        {
            //Logger.Log($"[EDI] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            var ser = new XmlSerializer(typeof(MailboxResponse));
            retRes returnedResult = null;

            returnedResult = Client?.listMBAllEx(AppConfigHandler.conf.EdiUser, AppConfigHandler.conf.EdiPassword, getBinaryData);

            if (returnedResult == null) Error("Нет соединения с edisoft");

            if (returnedResult?.res == "00000000")
                return ((MailboxResponse)ser.Deserialize(new StringReader(returnedResult.cnt))).DocumentInfo;
            else
                MessageBox.Show(ResponseErrorHandler(returnedResult));

            return null;
        }


        internal static List<DocumentInfo> ListMBEx(string partnerILN
            , string documentType
            , string documentVersion
            , string documentStandard
            , string documentTest
            , string dateFrom
            , string dateTo
            , string itemFrom
            , string itemTo
            , string documentStatus
            , int timeout = 5000)
        {
            //Logger.Log($"[EDI] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            var ser = new XmlSerializer(typeof(MailboxResponse));
            retRes returnedResult = null;

            returnedResult = Client?.listMBEx(AppConfigHandler.conf.EdiUser, AppConfigHandler.conf.EdiPassword, partnerILN, documentType, documentVersion, documentStandard, documentTest, dateFrom, dateTo, itemFrom, itemTo, documentStatus, timeout);

            if (returnedResult == null) Error("Нет соединения с edisoft");

            if (returnedResult?.res == "00000000")            
                return ((MailboxResponse)ser.Deserialize(new StringReader(returnedResult.cnt))).DocumentInfo;            
                
            else
                MessageBox.Show(ResponseErrorHandler(returnedResult));

            return null;
        }
    }

}
