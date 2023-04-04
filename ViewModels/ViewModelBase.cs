using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViewModels
{
    public class ViewModelBase: INotifyPropertyChanged
    {
        public void ShowChildWindow(Window window)
        {
            window.Show();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        
        [Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
