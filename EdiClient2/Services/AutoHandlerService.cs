using EdiClient.AppSettings;
using EdiClient.ViewModel;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EdiClient.Services
{
    public static class AutoHandlerService
    {
        internal static void Process()
        {
            // выхватим новые документы
            //DocumentRepository.GetNewOrders(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // для всех документов в базе
            //foreach (var doc in DocumentRepository.GetDocuments(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow))
            //{
                // если doc не сфэйлиный и готов к созданию в трейдере то добавим в трейдер
                //if (doc.IsReadyToTrader && !doc.IsInTrader && !doc.IsFailed) DocumentRepository.CreateTraderDocument(doc.ID);
                // если doc не сфэйлиный и готов к отправке как ответ на заказ то отправляем его
                //if (doc.IsReadyToOrdrsp && doc.IsInTrader && !doc.IsFailed) DocumentRepository.SendOrdrsp(doc);
                // если doc не сфэйлиный и готов к отправке как уведомление об отгрузки то отправляем его
                //if (doc.IsReadyToDesadv && doc.IsInTrader && doc.IsOrdrsp && !doc.IsFailed) DocumentRepository.SendDesadv(doc);
            //}
        }

        internal static void RegisterAutoHandler()
        {
            //if (AppConfig.EdiTimeout != null)
            //    if (AppConfig.EdiTimeout > 0)
            //    {
            //        Task.Factory.StartNew(() =>
            //        {
            //            while (true)
            //            {
            //                if (AppConfig.EnableAutoHandler == true
            //                    || AppConfig.EdiTimeout != null
            //                    || AppConfig.EdiTimeout <= 0) { break; }
            //                Process();
            //                Thread.Sleep((int)AppConfig.EdiTimeout * 1000 * 60);
            //            }
            //        });
            //    }
        }
    }
}
