using EdiClient.Model;
using EdiClient.ViewModel.Common;
using EdiClient.Services;
using Microsoft.Win32;
using System.ComponentModel;
using EdiClient.Services.Repository;
using System;
using System.Reflection;
using System.Windows;

namespace EdiClient.ViewModel.Ordrsp
{
    public class OrdrspListViewModel : ListViewModel<DocumentOrderResponse>, INotifyPropertyChanged
    {
        public override bool IsButtonEnabled => !SelectedItem?.IsInEdiAsOrdrsp ?? false;

        public OrdrspListViewModel()
        {
        }        

        public override void Refresh()
        {
            try
            {
                if (EdiService.SelectedRelationship == null) { Utilites.Error("Необходимо выбрать клиента"); return; }
                base.Refresh();
                Documents = OrderResponseRepository.GetOrdrsp(DateFrom, DateTo);
            }
            catch (Exception ex)
            {
                Utilites.Error(ex);
            }
        }

        public override void ToEdi(object o = null)
        {
            try
            {
                if (EdiService.SelectedRelationship == null) { Utilites.Error("Необходимо выбрать клиента"); return; }
                base.ToEdi();
                if (SelectedItem != null)
                {
                    OrderResponseRepository.SendOrdrsp(SelectedItem);
                    Refresh();
                }
            }
            catch (Exception ex)
            {
                Utilites.Error(ex);
            }
        }
        
        public override void SaveToXml(object o = null)
        {
            try
            {
                if (EdiService.SelectedRelationship == null) { Utilites.Error("Необходимо выбрать клиента"); return; }
                base.SaveToXml();
                if (SelectedItem != null)
                {
                    SaveFileDialog sfd = new SaveFileDialog
                    {
                        FileName = $"{SelectedItem.OrderResponseHeader.OrderResponseNumber}",
                        FilterIndex = 0,
                        Filter = "XML-файл (*.xml)|*.xml|Текстовый файл (*.txt)|*.txt",
                        InitialDirectory = "C:\\edi_docs\\ordrsp\\"
                    };
                    bool saveResultOk = sfd.ShowDialog().Value;
                    if (saveResultOk) XmlService<DocumentOrderResponse>.Serialize(SelectedItem, sfd.FileName);
                    Refresh();
                }
            }
            catch (Exception ex)
            {
                Utilites.Error(ex);
            }
        }

    }
}
