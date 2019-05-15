using EdiClient.Model;
using System;
using System.IO;
using System.Xml;
using System.Windows;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;

namespace EdiClient.Services
{

    internal static class XmlService<TModel>
    {
        internal static List<TModel> Deserialize(string rawDocument)
        {
            List<TModel> Documents = new List<TModel>();
            XmlSerializer ser = new XmlSerializer(typeof(TModel));
            var stream = new StringReader(rawDocument);
            
            using (XmlReader reader = XmlReader.Create(stream))
            {                
                Documents.Add((TModel)ser.Deserialize(reader));
            }

            return Documents;
        }

        /// <summary>
        /// Сериализует документ в XML-файл
        /// </summary>
        /// <param name="order">сериализуемый документ</param>
        /// <param name="OutPath">путь, куда сохраняется XML-файл</param>
        /// <returns>возвращает: true - успешно сохранён, false - ошибка</returns>
        internal static bool Serialize(TModel order, string OutPath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(TModel));
            try
            {
                using (XmlWriter writer = XmlWriter.Create(OutPath)) ser.Serialize(writer, order);
            }
            catch (Exception ex)
            {
                Utilites.Error(ex);
                return false;
            }
            return true;
        }

        internal static string Serialize(TModel order)
        {
            XmlSerializer ser = new XmlSerializer(typeof(TModel));
            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, new XmlWriterSettings() { Encoding = Encoding.UTF8 }))
            {
                ser.Serialize(writer, order);
            }
            return builder.ToString();
        }

    }
}
