using System.Collections.Generic;
using System.Data;
using SistemaReservas.Data;
using SistemaReservas.Models;
using SistemaReservas.MVVM;


namespace SistemaReservas.ViewModels
{
    public class AulaViewModel : ViewModelBase
    {


        private readonly AulaRepository repo;



        public DataTable AulasDataTable { get; set; }



        public List<Aula> Aulas { get; set; }



        public AulaViewModel()
        {

            repo =
                new AulaRepository();


            CargarAulasObjetos();

        }



        private void CargarAulasObjetos()
        {

            Aulas =
                repo.ObtenerAulasObjetos();


            OnPropertyChanged(nameof(Aulas));

        }



        public void CargarAulasTabla()
        {

            AulasDataTable =
                repo.ObtenerAulasDataTable();


            OnPropertyChanged(nameof(AulasDataTable));

        }


    }
}