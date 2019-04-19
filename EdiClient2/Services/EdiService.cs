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

namespace EdiClient.Services
{
    internal static class EdiService
    {
        internal static EDIWebServicePortTypeClient Client { get; set; }
        internal static ServiceEndpoint endPoint;
        internal static EndpointAddress address;
        private static string Name { get; set; }
        private static string Password { get; set; }

        public static List<Relation> Relationships { get; set; }
        public static Relation SelectedRelationship { get; set; }
        public static int RelationshipCount { get; set; }

        internal static void UpdateData()
        {
            Relationships = GetRelationships().Where(x => x.documentType == "ORDER").ToList() ?? throw new Exception("При загрузке связей возникла ошибка");
            SelectedRelationship = SelectedRelationship ?? (Relationships[0] ?? throw new Exception("Не выбрана связь с покупателем"));
            RelationshipCount = Relationships.Count;
        }

        internal static EDIWebServicePortTypeClient Configure(EndpointAddress _address = null)
        {
            Client = new EDIWebServicePortTypeClient();
            endPoint = Client.Endpoint;
            if (_address != null)
            {
                address = _address;
                Client.Endpoint.Address = address;
            }
            Name = AppSettings.AppConfig.Edi_User;
            Password = AppSettings.AppConfig.Edi_Password;
            UpdateData();
            return Client;
        }

        internal static EDIWebServicePortTypeClient Configure(string _endPointConfigurationName, EndpointAddress _endpointAddress)
        {
            Client = new EDIWebServicePortTypeClient(_endPointConfigurationName, _endpointAddress);
            Name = AppSettings.AppConfig.Edi_User;
            Password = AppSettings.AppConfig.Edi_Password;
            return Client;
        }

        internal static string ResponseErrorHandler(retRes res)
        {
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

        /// <summary>
        /// С помощью данного метода можно узнать информацию о компании, заведенной на платформе Эдисофт по одному из параметров: ИНН, КПП, ОГРН, GUID.
        /// </summary>
        /// <param name="inn">ИНН</param>
        /// <param name="kpp">КПП</param>
        /// <param name="ogrn">ОГРН</param>
        /// <param name="fnsId">ФНСИД</param>
        /// <param name="gln">GLN</param>
        /// <returns></returns>
        internal static OrganizationInfo OrganizationInfo(string inn, string kpp, string ogrn, string fnsId, string gln)
        {
            //LogService.Log($"[INFO] [EDI-METHOD] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
            retRes returnedResult = null;
            XmlSerializer serializer = new XmlSerializer(typeof(OrganizationInfo));

            returnedResult = Client.organizationInfo(Name, Password, inn, kpp, ogrn, fnsId, gln);

            if (returnedResult == null) throw new Exception("Нет соединения с edisoft");

            if (returnedResult.res == "00000000")
                return (OrganizationInfo)serializer.Deserialize(new StringReader(returnedResult.cnt));
            else
                MessageBox.Show(ResponseErrorHandler(returnedResult));

            return null;
        }

        internal static List<Relation> GetRelationships(int timeout = 5000)
        {
            //LogService.Log($"[INFO] [EDI-METHOD] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
            var ser = new XmlSerializer(typeof(RelationResponse));
            retRes returnedResult = null;
            //var s = Client?.State;
            returnedResult = Client?.relationships(Name, Password, timeout);

            if (returnedResult == null) throw new Exception("Запрос связей не дал результатов");

            if (returnedResult?.res == "00000000")
                return ((RelationResponse)ser.Deserialize(new StringReader(returnedResult.cnt))).Relations;
            else
                MessageBox.Show(ResponseErrorHandler(returnedResult));

            return null;
        }

        /// <summary>
        /// Метод, обеспечивающий получение документов. Документ, имеющий статус «Прочтённый» (Read), забрать нельзя.
        /// </summary>
        /// <param name="partnerILN">ID партнера, которому был послан документ.</param>
        /// <param name="documentType">Тип документа(напр.INVOICE).</param>
        /// <param name="trackingId">Идентификатор документа в ПЛАТФОРМЕ EDISOFT.</param>
        /// <param name="documentStandard">Стандарт документа(напр.EDIFACT, XML).</param>
        /// <param name="changeDocumentStatus">Изменить статус документа(установить прочитанным) – не работает</param>
        /// <param name="timeout">Таймаут на выполнение вызова метода(мс). Тип : Integer</param>
        /// <returns></returns>
        internal static List<TModel> Receive<TModel>(string partnerILN, string documentType, string trackingId, string documentStandard, string changeDocumentStatus, int timeout = 5000)
        {
            //LogService.Log($"[INFO] [EDI-METHOD] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
            var ser = new XmlSerializer(typeof(TModel));
            retRes returnedResult = null;

            returnedResult = Client?.receive(Name, Password, partnerILN, documentType, trackingId, documentStandard, changeDocumentStatus, timeout);

            if (returnedResult == null) throw new Exception("Нет соединения с edisoft");

            if (returnedResult?.res == "00000000")
                return XmlService<TModel>.Deserialize(returnedResult.cnt);
            else
                MessageBox.Show(ResponseErrorHandler(returnedResult));

            return null;
        }

        /// <summary>
        /// Данный метод используется для отправки документов.
        /// </summary>
        /// <param name="partnerILN">ID партнера, которому будет посылаться документ.</param>
        /// <param name="documentType">Тип документа (напр.INVOICE).</param>
        /// <param name="documentVersion">Версия спецификации (напр.EDISOFTV0R1).</param>
        /// <param name="documentStandard">Стандарт документа (напр.EDIFACT, XML).</param>
        /// <param name="documentTest">Статус документа (T – тест, P – продуктивный).</param>
        /// <param name="controlNumber">Контрольный номер документа</param>
        /// <param name="documentContent">Содержание документа</param>
        /// <param name="timeout">Таймаут на выполнение вызова метода(мс)</param>
        internal static void Send(string partnerILN, string documentType, string documentVersion, string documentStandard, string documentTest, string controlNumber, string documentContent, int timeout = 5000)
        {
            //LogService.Log($"[INFO] [EDI-METHOD] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
            retRes returnedResult = null;

            returnedResult = Client.send(Name, Password, partnerILN, documentType, documentVersion, documentStandard, documentTest, controlNumber, documentContent, timeout);

            if (returnedResult?.res != "00000000" && returnedResult != null)
            {
                MessageBox.Show("КОД " + returnedResult.res + "\n\n" + returnedResult.cnt);
            }
        }

        internal static List<DocumentInfo> ListMBAll(bool getBinaryData = false)
        {
            //LogService.Log($"[INFO] [EDI-METHOD] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
            var ser = new XmlSerializer(typeof(MailboxResponse));
            retRes returnedResult = null;

            returnedResult = Client?.listMBAllEx(Name, Password, getBinaryData);

            if (returnedResult == null) throw new Exception("Нет соединения с edisoft");

            if (returnedResult?.res == "00000000")
                return ((MailboxResponse)ser.Deserialize(new StringReader(returnedResult.cnt))).DocumentInfo;
            else
                MessageBox.Show(ResponseErrorHandler(returnedResult));

            return null;
        }

        /// <summary>
        /// Метод возвращает статус документов, которые были доставлены пользователю ПЛАТФОРМЫ EDISOFT
        /// </summary>
        /// <param name="partnerILN">ID партнера, от которого был получен документ</param>
        /// <param name="documentType">Тип документа (напр. INVOICE).</param>
        /// <param name="documentVersion">Версия спецификации (напр. EDISOFTV0R1).</param>
        /// <param name="documentStandard">Стандарт документа (напр. EDIFACT,XML).</param>
        /// <param name="documentTest">Статус документа (T – тест, P – продуктивный).</param>
        /// <param name="dateFrom">Дата с</param>
        /// <param name="dateTo">Дата до</param>
        /// <param name="itemFrom">???</param>
        /// <param name="itemTo">???</param>
        /// <param name="documentStatus">Статус документа N – только новые док-ты R – только прочтенные документы    Любое другое или пустое вернёт все</param>
        /// <param name="timeout">Таймаут на выполнение вызова метода (мс)</param>
        /// <returns>Cтатус документов</returns>
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
            //LogService.Log($"[INFO] [EDI-METHOD] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
            var ser = new XmlSerializer(typeof(MailboxResponse));
            retRes returnedResult = null;

            returnedResult = Client?.listMBEx(Name, Password, partnerILN, documentType, documentVersion, documentStandard, documentTest, dateFrom, dateTo, itemFrom, itemTo, documentStatus, timeout);

            if (returnedResult == null) throw new Exception("Нет соединения с edisoft");

            if (returnedResult?.res == "00000000")            
                return ((MailboxResponse)ser.Deserialize(new StringReader(returnedResult.cnt))).DocumentInfo;            
                
            else
                MessageBox.Show(ResponseErrorHandler(returnedResult));

            return null;
        }
    }

}
