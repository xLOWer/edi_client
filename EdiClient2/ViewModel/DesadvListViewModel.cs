using EdiClient.Model;
using EdiClient.ViewModel.Common;
using EdiClient.Services;
using Microsoft.Win32;
using System.ComponentModel;
using EdiClient.Services.Repository;
using System.Reflection;
using System;
using System.Windows;

namespace EdiClient.ViewModel.Desadv
{
    public class DesadvListViewModel : ListViewModel<DocumentDespatchAdvice>, INotifyPropertyChanged
    {
        public override bool IsButtonEnabled => !SelectedItem?.IsInEdiAsDesadv ?? false;

        public DesadvListViewModel()
        {
        }

        public override void Refresh()
        {
            try
            {
                base.Refresh();
                DespatchAdviceRepository.UpdateData(DateFrom, DateTo);
                Documents = DespatchAdviceRepository.GetDesadv(DateFrom, DateTo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[ERROR] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}\n\n{ex?.Message}\n\n{ex?.TargetSite}\n\n{ex.InnerException?.Message}\n\n{ex.StackTrace}");
            }
        }

        public override void ToEdi(object o)
        {
            try
            {
                base.ToEdi();
                if (SelectedItem != null)
                {
                    DespatchAdviceRepository.SendDesadv(SelectedItem);
                    Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[ERROR] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}\n\n{ex?.Message}\n\n{ex?.TargetSite}\n\n{ex.InnerException?.Message}\n\n{ex.StackTrace}");
            }
        }

        public override void SaveToXml(object o)
        {
            try
            {
                base.SaveToXml();
                if (SelectedItem != null)
                {
                    SaveFileDialog sfd = new SaveFileDialog
                    {
                        FileName = $"DESADV_{SelectedItem.DespatchAdviceHeader.DespatchAdviceNumber}",
                        FilterIndex = 0,
                        Filter = "XML-файл (*.xml)|*.xml|Текстовый файл (*.txt)|*.txt",
                        InitialDirectory = "C:\\edi_docs\\Desadv\\"
                    };
                    bool saveResultOk = sfd.ShowDialog().Value;
                    if (saveResultOk) XmlService<DocumentDespatchAdvice>.Serialize(SelectedItem, sfd.FileName);
                    Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[ERROR] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}\n\n{ex?.Message}\n\n{ex?.TargetSite}\n\n{ex.InnerException?.Message}\n\n{ex.StackTrace}");
            }
        }

    }
}
