﻿using EdiClient.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace EdiClient.Services.Repository
{
    internal static class ReceivingAdviceRepository
    {
        public static List<Model.WebModel.RelationResponse.Relation> Relationships => EdiService.Relationships;
        public static Model.WebModel.RelationResponse.Relation SelectedRelationship => EdiService.SelectedRelationship;
        public static int RelationshipCount => EdiService.RelationshipCount;

        public static List<Model.WebModel.DocumentInfo> NewAdvices { get; set; }
        public static List<DocumentReceivingAdvice> Advices { get; set; }
        public static List<Task> NativeTaskList = new List<Task>();

        internal static void UpdateData(DateTime dateFrom, DateTime dateTo)
        {
            NewAdvices = new List<Model.WebModel.DocumentInfo>();

            NewAdvices = EdiService.ListMBEx(
                                            SelectedRelationship?.partnerIln
                                            , SelectedRelationship?.documentType
                                            , ""
                                            , ""
                                            , ""
                                            , $"{dateFrom.Year}-{dateFrom.Month}-{dateFrom.Day}"
                                            , $"{dateTo.Year}-{dateTo.Month}-{dateTo.Day}"
                                            , ""
                                            , ""
                                            , "") ?? throw new Exception("При загрузке новых уведомлений об отгрузке возникла ошибка");
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} args:{LogService.FormatArgsArray(MethodBase.GetCurrentMethod().GetGenericArguments())}", 2);
        }

        public static List<DocumentReceivingAdvice> GetRecadv(DateTime dateFrom, DateTime dateTo)
        {
            Advices = new List<DocumentReceivingAdvice>();
            if (RelationshipCount > 0)
                foreach (var rel in Relationships)
                    if (NewAdvices.Count > 0)
                        foreach (var order in NewAdvices)
                            NativeTaskList.Add(Task.Factory.StartNew(()
                                => AddAdvices(rel.partnerIln, rel.documentType, order.trackingId, rel.documentStandard, order.partneriln)));

            Task.WaitAll(NativeTaskList.ToArray());
            NativeTaskList.Clear();
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} args:{LogService.FormatArgsArray(MethodBase.GetCurrentMethod().GetGenericArguments())}", 2);
            return Advices;
        }

        private static void AddAdvices(string relPartnerIln, string relDocumentType, string newOrderTrackingId, string relDocumentStandard, string orderPartnerIln)
        {
            if (relPartnerIln == orderPartnerIln)
                Advices.AddRange(EdiService.Receive<DocumentReceivingAdvice>(relPartnerIln, relDocumentType, newOrderTrackingId, relDocumentStandard, ""));
        }

    }
}
