using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SistemaReservas.Models;


namespace SistemaReservas.Data
{
    public class AulaRepository
    {


        // ESCENARIO DESCONTECTADO
        public System.Data.DataTable ObtenerAulasDataTable()
        {

            System.Data.DataTable tabla =
                new System.Data.DataTable();


            using (SqlConnection cn =
                new SqlConnection(Conexion.CadenaConexion))
            {

                string sql =
                    "SELECT AulaId, Nombre, Capacidad FROM Aulas";


                SqlDataAdapter adapter =
                    new SqlDataAdapter(sql, cn);


                adapter.Fill(tabla);

            }


            return tabla;

        }



        // ESCENARIO CONECTADO
        public List<Aula> ObtenerAulasObjetos()
        {

            List<Aula> lista =
                new List<Aula>();


            using (SqlConnection cn =
                new SqlConnection(Conexion.CadenaConexion))
            {

                cn.Open();


                string sql =
                    "SELECT AulaId, Nombre, Capacidad FROM Aulas";


                SqlCommand cmd =
                    new SqlCommand(sql, cn);



                SqlDataReader reader =
                    cmd.ExecuteReader();



                while (reader.Read())
                {

                    Aula aula =
                        new Aula();


                    aula.AulaId =
                        Convert.ToInt32(reader["AulaId"]);


                    aula.Nombre =
                        reader["Nombre"].ToString();


                    aula.Capacidad =
                        Convert.ToInt32(reader["Capacidad"]);



                    lista.Add(aula);

                }


                cn.Close();

            }


            return lista;

        }


    }
}