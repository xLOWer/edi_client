﻿using EdiClient.Model;
using EdiClient.ViewModel.Common;
using EdiClient.Services;
using Microsoft.Win32;
using System.ComponentModel;
using EdiClient.Services.Repository;
using System.Reflection;
using System;
using System.Windows;

namespace EdiClient.ViewModel.Recadv
{
    public class RecadvListViewModel : ListViewModel<DocumentReceivingAdvice>, INotifyPropertyChanged
    {
        public RecadvListViewModel()
        {
        }
        
        public override void Refresh()
        {
            try
            {
                ReceivingAdviceRepository.UpdateData(DateFrom, DateTo);
                base.Refresh();
                Documents = ReceivingAdviceRepository.GetRecadv(DateFrom, DateTo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[ERROR] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}\n\n{ex?.Message}\n\n{ex?.TargetSite}\n\n{ex.InnerException?.Message}\n\n{ex.StackTrace}");
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
                        FileName = $"from{SelectedItem.DocumentParties.Sender.ILN}__to{SelectedItem.DocumentParties.Receiver.ILN}",
                        FilterIndex = 0,
                        Filter = "XML-файл (*.xml)|*.xml|Текстовый файл (*.txt)|*.txt",
                        InitialDirectory = "C:\\edi_docs\\Recadv\\"
                    };
                    bool saveResultOk = sfd.ShowDialog().Value;
                    if (saveResultOk) XmlService<DocumentReceivingAdvice>.Serialize(SelectedItem, sfd.FileName);
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
