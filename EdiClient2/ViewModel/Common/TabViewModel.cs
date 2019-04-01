using System;
using System.Reflection;
using System.Windows.Controls;

namespace EdiClient.ViewModel.Common
{
    public struct TabViewModel
    {
        public string Title { get; set; }
        public object Content { get; set; }
        public Type View { get; set; }

        public TabViewModel(Type view, string title)
        {
            Title = title;
            View = view;
            Content = new Frame()
            {
                Content = // суём в контейнер созданный экземпляр
                    (view as Type) // получаем тип вьюхи
                    .GetConstructor(BindingFlags.Instance | BindingFlags.Public // ищем public констркутор
                                    , null
                                    , CallingConventions.HasThis // имеющий возможность создавать экземпляры
                                    , new Type[0] { } // с одним параметром типа как наш документ
                                    , null)
                    .Invoke(new object[0] { } // вызываем конструктор подставляя документ в параметр
                )
            };
        }
        
    }  
}