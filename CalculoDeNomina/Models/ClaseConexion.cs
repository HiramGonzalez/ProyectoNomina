using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace CalculoDeNomina.Models
{
    public abstract class ClaseConexion
    {
        private readonly string connectionString;

        public ClaseConexion()
        {
            connectionString = ConfigurationManager.ConnectionStrings["connCalculosDeNominaDB"].ToString();
        }

        protected SqlConnection ObtenerConexion()
        {
            return new SqlConnection(connectionString);
        }
    }
}