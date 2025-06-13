using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CalculoDeNomina.Models
{
    public abstract class ConexionMaestra: ClaseConexion
    {
        protected List<SqlParameter> parametros;

        protected int ExecuteNonQuery(string transactSQL)
        {
            using (var conexion = ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexion;
                    comando.CommandText = transactSQL;
                    comando.CommandType = CommandType.Text;
                    foreach(SqlParameter parametro in parametros)
                    {
                        comando.Parameters.Add(parametro);
                    }
                    int result = comando.ExecuteNonQuery();
                    parametros.Clear();
                    return result;
                }
            }
        }


        protected DataTable ExecuteReader(string transactSQL)
        {
            using (var conexion = ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexion;
                    comando.CommandText = transactSQL;
                    comando.CommandType = CommandType.Text;
                    SqlDataReader reader = comando.ExecuteReader();
                    using (var tabla = new DataTable())
                    {
                        tabla.Load(reader);
                        reader.Dispose();
                        return tabla;
                    }
                }
            }
        }


        protected DataTable ExecuteReaderParam(string transactSQL)
        {
            using (var conexion = ObtenerConexion())
            {
                conexion.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexion;
                    comando.CommandText = transactSQL;
                    comando.CommandType = CommandType.Text;
                    foreach(SqlParameter parametro in parametros)
                    {
                        comando.Parameters.Add(parametro);
                    }

                    SqlDataReader reader = comando.ExecuteReader();
                    using (var tabla = new DataTable())
                    {
                        tabla.Load(reader);
                        reader.Dispose();
                        return tabla;
                    }
                }
            }
        }
    }
}