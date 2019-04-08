using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EdiClient.Services
{
    [Magic]
    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        protected virtual void RaisePropertyChanged(string propName)
        {
            var e = PropertyChanged;
            if (e != null)
                e( this, new PropertyChangedEventArgs( propName ) ); // некоторые из нас здесь используют Dispatcher, для безопасного взаимодействия с UI thread
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

class MagicAttribute : Attribute { }