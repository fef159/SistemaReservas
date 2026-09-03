using Microsoft.Data.SqlClient;
using SistemaReservas.Models;

namespace SistemaReservas.Data
{
    public class UsuarioRepository
    {

        public Usuario ValidarUsuario(string username, string password)
        {
            Usuario usuario = null;


            using (SqlConnection cn =
                new SqlConnection(Conexion.CadenaConexion))
            {

                cn.Open();


                string sql = @"SELECT *
                               FROM Usuarios
                               WHERE Username=@username
                               AND Password=@password";


                SqlCommand cmd = new SqlCommand(sql, cn);


                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);



                SqlDataReader dr = cmd.ExecuteReader();



                if (dr.Read())
                {

                    usuario = new Usuario()
                    {

                        UsuarioId = Convert.ToInt32(dr["UsuarioId"]),

                        Username = dr["Username"].ToString(),

                        Password = dr["Password"].ToString(),

                        NombreCompleto = dr["NombreCompleto"].ToString()

                    };

                }


                cn.Close();

            }


            return usuario;

        }

    }
}