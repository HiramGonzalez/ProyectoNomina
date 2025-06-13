using CalculoDeNomina.Models;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace CalculoDeNomina.Controllers
{
    [Authorize]
    public class HistorialNominasController : Controller
    {
        // GET: HistorialNominas
        public ActionResult HistorialNominas()
        {
            using (var context = new CalculosDeNominaEntities())
            {
                List<CalculosDeNomina> registrosNomina = context.CalculosDeNomina.ToList();
                return View(registrosNomina);
            }
        }

        // Ir a la página de detalle del calculo de nomina
        public ActionResult DetalleNomina(int id)
        {
            using (var context = new CalculosDeNominaEntities())
            {
                var registro = context.CalculosDeNomina.FirstOrDefault(nomina => nomina.ID == id);
                if(registro == null) return HttpNotFound();

                ViewBag.TablaExcel = GenerarHTMLArchivoExcel(registro.ArchivoNomina);

                return View(registro);
            }
        }

        // Ir a la página para crear un nuevo registro de nomina
        public ActionResult PaginaAgregarCalculoNomina()
        {
            ViewBag.ErrorNombreNomina = false;
            return View("AgregarCalculoDeNomina");
        }


        // Metodo para agregar un registro de calculo de nomina
        [HttpPost]
        public ActionResult AgregarCalculoNomina(string nombreCalculo, HttpPostedFileBase archivo)
        {
            using (var context = new CalculosDeNominaEntities())
            {
                CalculosDeNomina nuevoRegistro = new CalculosDeNomina();
                if (nombreCalculo.IsEmpty())
                {
                    ViewBag.ErrorNombreNomina = true;
                    return View("AgregarCalculoDeNomina");
                }

                nuevoRegistro.NombreCalculo = nombreCalculo;
                nuevoRegistro.FechaCalculo = DateTime.Now;

                if(archivo != null)
                {
                    byte[] archivoBytes = ConvertirArchivoExcelABytes(archivo);
                    nuevoRegistro.ArchivoNomina = archivoBytes;
                    nuevoRegistro.FilasAfectadas = ContarFilasDeArchivo(archivoBytes);
                    nuevoRegistro.Estado = true;
                }

                context.CalculosDeNomina.Add(nuevoRegistro);
                context.SaveChanges();
                return RedirectToAction("HistorialNominas");
            }
        }


        // Metodo para actualizar un registro de calculo de nomina
        [HttpPost]
        public ActionResult ActualizarNomina(int id, HttpPostedFileBase archivo)
        {
            using (var context = new CalculosDeNominaEntities())
            {
                var registro = context.CalculosDeNomina.FirstOrDefault(nomina => nomina.ID == id);
                if (registro == null) return HttpNotFound();

                if (archivo != null && archivo.ContentLength > 0)
                {
                    byte[] archivoBytes = ConvertirArchivoExcelABytes(archivo);
                    registro.ArchivoNomina = archivoBytes;
                    registro.FilasAfectadas = ContarFilasDeArchivo(archivoBytes);
                    registro.Estado = true;
                    context.SaveChanges();
                }

                List<CalculosDeNomina> registrosNomina = context.CalculosDeNomina.ToList();
                return View("HistorialNominas", registrosNomina);
            }
        }


        // Método que se utiliza para contar las filas del archivo Excel
        private int ContarFilasDeArchivo(byte[] archivo)
        {
            using (var ms = new MemoryStream(archivo))
            using (var libro = new XLWorkbook(ms))
            {
                var hoja = libro.Worksheets.First();
                return hoja.RowsUsed().Count();
            }
        }

        // Metodo que se usa para generar el HTML para mostrar el contenido del archivo Excel
        private string GenerarHTMLArchivoExcel(byte[] archivo)
        {
            if (archivo == null) return "<p></p>";

            StringBuilder html = new StringBuilder();
            html.Append("<table class='table' >");

            using (var ms = new MemoryStream(archivo))
            using (var libro = new XLWorkbook(ms))
            {
                var hoja = libro.Worksheets.First();
                foreach (var fila in hoja.RowsUsed())
                {
                    html.Append("<tr>");

                    foreach (var celda in fila.CellsUsed())
                    {
                        html.AppendFormat("<td>{0}</td>", celda.Value.ToString());
                    }

                    html.Append("</tr>");
                }
            }

            html.Append("</table>");
            return html.ToString();
        }


        // Metodo que convierte el archivo Excel a tipo byte[] para ser almacenado en la DB
        private byte[] ConvertirArchivoExcelABytes(HttpPostedFileBase archivo)
        {
            byte[] archivoBytes;
            using (var ms = new MemoryStream())
            {
                archivo.InputStream.CopyTo(ms);
                archivoBytes = ms.ToArray();
            }

            return archivoBytes;
        }



        public ActionResult VolverAlMenu()
        {
            return RedirectToAction("MainMenu", "MainMenu");
        }
    }
}