using System;
using System.Windows;

namespace EdiClient.Services
{
    public static class Utilites
    {
        internal static void Error(Exception ex)
        {
            LogService.Log($"===============================");
            var msg = $"Message: {ex.Message}\n\nSource: {ex.Source}\n\n{GetInnerExceptionMessage(ex)}\n\nTargetSite: {ex?.TargetSite}\n\n{ex.InnerException?.Message}\n\nStackTrace: {ex.StackTrace}";                
            LogService.Log(msg);
            LogService.Log($"===============================");
            DevExpress.Xpf.Core.DXMessageBox.Show( msg, "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error );
        }


        internal static void Error(string text)
        {
            LogService.Log($"===============================");
            LogService.Log($"[ОШИБКА] {text}");
            LogService.Log($"===============================");
            DevExpress.Xpf.Core.DXMessageBox.Show( $"[ОШИБКА] {text}", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error );
        }


        private static string GetInnerExceptionMessage(Exception ex)
            => ex.InnerException != null ? ex.Message + GetInnerExceptionMessage( ex.InnerException ) : $"\ninner: {ex.Message}\n{ex.Data}\n{ex.Source}\n{ex.TargetSite}\n===========\n";


        public static string Time { get; set; }

    }
}