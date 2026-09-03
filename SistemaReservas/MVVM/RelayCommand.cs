using System;
using System.Windows.Input;


namespace SistemaReservas.MVVM
{

    public class RelayCommand : ICommand
    {


        private readonly Action<object> ejecutar;



        public RelayCommand(Action<object> ejecutar)
        {

            this.ejecutar = ejecutar;

        }



        public bool CanExecute(object parameter)
        {
            return true;
        }



        public void Execute(object parameter)
        {

            ejecutar(parameter);

        }



        public event EventHandler CanExecuteChanged
        {

            add { }

            remove { }

        }


    }

}