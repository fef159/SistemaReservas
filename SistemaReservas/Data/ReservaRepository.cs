using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaReservas.Models;


namespace SistemaReservas.Data
{
    public class ReservaRepository
    {


        // ESCENARIO DESCONTECTADO
        public DataTable ObtenerReservasDataTable()
        {

            DataTable tabla =
                new DataTable();


            using (SqlConnection cn =
                new SqlConnection(Conexion.CadenaConexion))
            {


                string sql = @"
                    SELECT 
                        R.ReservaId,
                        U.NombreCompleto AS Usuario,
                        A.Nombre AS Aula,
                        R.Fecha,
                        R.Hora,
                        R.Motivo
                    FROM Reservas R
                    INNER JOIN Usuarios U
                    ON R.UsuarioId = U.UsuarioId
                    INNER JOIN Aulas A
                    ON R.AulaId = A.AulaId";


                SqlDataAdapter adapter =
                    new SqlDataAdapter(sql, cn);


                adapter.Fill(tabla);

            }


            return tabla;

        }





        // ESCENARIO CONECTADO
        public List<Reserva> ObtenerReservasObjetos()
        {

            List<Reserva> lista =
                new List<Reserva>();



            using (SqlConnection cn =
                new SqlConnection(Conexion.CadenaConexion))
            {

                cn.Open();



                string sql = @"
                    SELECT 
                        ReservaId,
                        AulaId,
                        UsuarioId,
                        Fecha,
                        Hora,
                        Motivo
                    FROM Reservas";



                SqlCommand cmd =
                    new SqlCommand(sql, cn);



                SqlDataReader reader =
                    cmd.ExecuteReader();



                while (reader.Read())
                {

                    Reserva reserva =
                        new Reserva();


                    reserva.ReservaId =
                        Convert.ToInt32(reader["ReservaId"]);



                    reserva.AulaId =
                        Convert.ToInt32(reader["AulaId"]);



                    reserva.UsuarioId =
                        Convert.ToInt32(reader["UsuarioId"]);



                    reserva.Fecha =
                        Convert.ToDateTime(reader["Fecha"]);



                    reserva.Hora =
                        (TimeSpan)reader["Hora"];



                    reserva.Motivo =
                        reader["Motivo"].ToString();



                    lista.Add(reserva);

                }



                cn.Close();

            }



            return lista;

        }


    }
}