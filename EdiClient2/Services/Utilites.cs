﻿using System;
using System.Windows;

namespace EdiClient.Services
{
    public static class Utilites
    {
        internal static void Error(Exception ex)
        {
            MessageBox.Show( $"[ОШИБКА] {GetInnerExceptionMessage( ex )}\n\n{ex?.TargetSite}\n\n{ex.InnerException?.Message}\n\n{ex.StackTrace}",
                "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error );
        }


        internal static void Error(string text)
        {
            MessageBox.Show( $"[ОШИБКА] {text}", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error );
        }


        private static string GetInnerExceptionMessage(Exception ex)
            => ex.InnerException != null ? ex.Message + GetInnerExceptionMessage( ex.InnerException ) : $"\ninner: {ex.Message}";


        public static string Time { get; set; }

    }
}