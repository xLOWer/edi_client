using EdiClient.AppSettings;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;

// Общие сведения об этой сборке предоставляются следующим набором
// набора атрибутов. Измените значения этих атрибутов, чтобы изменить сведения,
// связанные со сборкой.
[assembly: AssemblyTitle("Клиент EDI "+ AppConfig.AppVersion)]
[assembly: AssemblyDescription("Программа для работы с Edisoft")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("WERA")]
[assembly: AssemblyProduct("EdiClient"+ AppConfig.AppVersion)]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, 
    ResourceDictionaryLocation.SourceAssembly 
)]

[assembly: AssemblyVersion(AppConfig.AppVersion)]
[assembly: AssemblyFileVersion(AppConfig.AppVersion)]
[assembly: NeutralResourcesLanguage("ru-RU")]

