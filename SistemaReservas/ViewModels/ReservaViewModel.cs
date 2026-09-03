using System.Collections.Generic;
using System.Data;
using SistemaReservas.Data;
using SistemaReservas.Models;
using SistemaReservas.MVVM;


namespace SistemaReservas.ViewModels
{
    public class ReservaViewModel : ViewModelBase
    {


        private readonly ReservaRepository repo;



        public DataTable ReservasDataTable { get; set; }



        public List<Reserva> Reservas { get; set; }




        public ReservaViewModel()
        {

            repo =
                new ReservaRepository();


            CargarReservasObjetos();

        }




        private void CargarReservasObjetos()
        {

            Reservas =
                repo.ObtenerReservasObjetos();


            OnPropertyChanged(nameof(Reservas));

        }




        public void CargarReservasTabla()
        {

            ReservasDataTable =
                repo.ObtenerReservasDataTable();


            OnPropertyChanged(nameof(ReservasDataTable));

        }


    }
}