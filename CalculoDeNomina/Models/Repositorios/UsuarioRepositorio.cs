using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CalculoDeNomina.Models.Repositorios
{
    public class UsuarioRepositorio: ConexionMaestra
    {
        private readonly CalculosDeNominaEntities _context;

        public UsuarioRepositorio()
        {
            _context = new CalculosDeNominaEntities();
        }

        // Método para realizar validación de Login a la DB
        public bool Login(string nombreUsuario, string password)
        {
            using (var conexion = ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexion;
                    comando.CommandText = "SELECT * FROM Usuario WHERE NombreUsuario = @nombreUsuario AND CONVERT(VARCHAR(MAX), DECRYPTBYPASSPHRASE('password', Password)) = @password";
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    comando.Parameters.AddWithValue("@password", password);
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Cache.Cache.id_usuario = reader.GetInt32(0);
                            }
                            return true;
                        } else
                        {
                            return false;
                        }
                    }
                }
            }
        }
    }
}