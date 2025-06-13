using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalculoDeNomina.Models
{
    public class DetalleNomina: ConexionMaestra
    {
        private int id;
        private string nombreCalculo;
        private DateTime? fechaCalculo = null;
        private int filasAfectadas = 0;
        private byte estado = 0;
        private byte[] archivoNomina = null;

        public int ID { get { return id; } set { id = value; } }
        public string NombreCalculo { get { return nombreCalculo; } set { nombreCalculo = value; } }
        public DateTime? FechaCalculo { get { return fechaCalculo; } set { fechaCalculo = value; } }
        public int FilasAfectadas { get { return filasAfectadas; } set { filasAfectadas = value; } }
        public byte Estado { get { return estado; } set { estado = value; } }
        public byte[] ArchivoNomina {  get { return archivoNomina; } set { archivoNomina = value; } }


        public DetalleNomina()
        {
            id = ID;
            nombreCalculo = NombreCalculo;
            fechaCalculo = FechaCalculo;
            filasAfectadas = FilasAfectadas;
            estado = Estado;
            archivoNomina = ArchivoNomina;
        }



    }
}