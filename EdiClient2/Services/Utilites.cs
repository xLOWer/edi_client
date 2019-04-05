using System;
using System.Windows;

namespace EdiClient.Services
{
    public static class Utilites
    {
        internal static void Error(Exception ex)
        {            
            MessageBox.Show($"[ERROR] {GetInnerExceptionMessage(ex)}\n\n{ex?.TargetSite}\n\n{ex.InnerException?.Message}\n\n{ex.StackTrace}");
        }
        private static string GetInnerExceptionMessage(Exception ex)
            => ex.InnerException != null ? ex.Message + GetInnerExceptionMessage(ex.InnerException) : $"\ninner: {ex.Message}";

        public static string Time { get; set; }
        
    }
}
