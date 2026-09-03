using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace SistemaReservas.MVVM
{

    public class ViewModelBase : INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;



        protected void OnPropertyChanged(
            [CallerMemberName] string nombre = null)
        {

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nombre));

        }


    }

}