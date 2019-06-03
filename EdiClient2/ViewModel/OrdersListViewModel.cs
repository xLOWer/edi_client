using EdiClient.Model;
using EdiClient.Services;
using EdiClient.Services.Repository;
using EdiClient.ViewModel.Common;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace EdiClient.ViewModel.Orders
{
    public class OrdersListViewModel : ListViewModel<DocumentOrder>, INotifyPropertyChanged
    {
        public override bool IsButtonEnabled => SelectedItem != null && !SelectedItem.IsInDatabase && !SelectedItem.IsFailed;

        public OrdersListViewModel()
        {
            try
            {
                OrdersRepository.UpdateData(DateFrom, DateTo);
            }
            catch (Exception ex) { Utilites.Error(ex); }
        }
        
        public override void Refresh()
        {
            try
            {
                OrdersRepository.UpdateData(DateFrom, DateTo);
                base.Refresh();
            }
            catch (Exception ex)
            {
                Utilites.Error(ex);
            }
        }

        public override void UpdateView()
        {
            if (EdiService.SelectedRelationship == null) { Utilites.Error("Необходимо выбрать клиента"); return; }
            IsCanRefresh = false;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                base.UpdateView();
                Documents = OrdersRepository.GetOrders(DateFrom, DateTo);
                if (Documents == null) return;
                if (Documents?.Count > 0)
                {
                    var docsAlredyInBufferTable = OrdersRepository.GetOrdersLocatedInBufferTable();
                    //var docsHasFailedDetails = OrdersRepository.GetOrdersFailed();

                    var docNums = "";

                    foreach (var doc in Documents)
                    {
                        doc.EdiIdDoc = "";
                        doc.EdiIdDoc = DbService.SelectSingleValue($"select ID from HPCSERVICE.EDI_DOC WHERE order_number = '{doc?.OrderHeader?.OrderNumber}'");
                        doc.TraderNumber = DbService.SelectSingleValue($"select CODE from ABT.DOC_JOURNAL WHERE id in (select id_trader from hpcservice.edi_doc where id = '{doc?.EdiIdDoc}')");
                        OrdersRepository.UpdateFailedDetails(doc?.EdiIdDoc);
                        if (doc == null) continue;
                        if (doc.OrderHeader == null) continue;
                        if (doc.OrderHeader.OrderNumber == null) continue;

                        docNums += $",'{doc.OrderHeader.OrderNumber}'";
                    }
                    docNums = docNums?.Trim(',');

                    var detailsFailed = OrdersRepository.GetDetailsFailed(docNums);

                    if (docsAlredyInBufferTable != null)
                        foreach (var doc in Documents)
                        {
                            if (docsAlredyInBufferTable.Contains(doc?.OrderHeader?.OrderNumber))
                            {
                                doc.IsInDatabase = true;
                            }
                            if (detailsFailed != null)
                                foreach (var line in doc.OrderLines.Lines)
                                {
                                    if (detailsFailed.Contains(line?.LineItem?.BuyerItemCode))
                                    {
                                        line.IsFailed = true;
                                        doc.IsFailed = true;
                                    }
                                }
                        }

                }
            }
            catch (Exception ex)
            {
                Utilites.Error(ex);
            }
            finally
            {
                watch.Stop();
                Time = ((double)(((double)watch.ElapsedMilliseconds) / 1000)).ToString() + " сек";
                IsCanRefresh = true;
            }
        }

        public override void SaveToXml(object o = null)
        {
            try
            {
                base.SaveToXml();
                if (SelectedItem != null)
                {
                    SaveFileDialog sfd = new SaveFileDialog
                    {
                        FileName = $"{SelectedItem.OrderHeader.OrderNumber}",
                        FilterIndex = 0,
                        Filter = "XML-файл (*.xml)|*.xml|Текстовый файл (*.txt)|*.txt",
                        InitialDirectory = "C:\\"
                    };
                    bool saveResultOk = sfd.ShowDialog().Value;
                    if (saveResultOk)
                    {
                        XmlService<DocumentOrder>.Serialize(SelectedItem, sfd.FileName);
                    }
                    Refresh();
                }
            }
            catch (Exception ex)
            {
                Utilites.Error(ex);
            }

            base.SaveToXml(o);
        }

        public override void CreateTraderDocument(object o = null)
        {

            base.CreateTraderDocument();
            if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.OrderHeader.OrderNumber))
            {
                OrdersRepository.CreateTraderDocument(SelectedItem.OrderHeader.OrderNumber);
                UpdateView();
            }
            else
                throw new Exception("Вы ничего не выбрали или выбранный документ не имеет номера");

        }

    }
}