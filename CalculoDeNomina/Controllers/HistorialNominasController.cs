using CalculoDeNomina.Models;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;


namespace CalculoDeNomina.Controllers
{
    [Authorize]
    public class HistorialNominasController : Controller
    {
        //public string fechaCalculo;
        //public string status;
        //public string cantidadFilas;
        //public string tablaExcel;

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
                //this.tablaExcel = GenerarHTMLArchivoExcel(registro.ArchivoNomina);
                //this.fechaCalculo = registro.FechaCalculo.ToString();
                //this.status = registro.Estado ? "Procesado" : "En proceso";
                //this.cantidadFilas = registro.FilasAfectadas.ToString();
                ViewBag.MostrarModal = false;

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


        // Metodo que se usa para el envio de correos
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EnviarCorreo(
            string correoDestinatario, 
            string correoCopia, 
            string asunto, 
            string fechaCalculo,
            string status,
            string cantidadFilas,
            string tablaExcel,
            string id,
            string mensaje = ""
            )
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("hiramagzzrdz@yahoo.com.mx");

                string[] destinatarios = SepararCadenaDeCorreos(correoDestinatario);
                foreach (string destinatario in destinatarios)
                {
                    correo.To.Add(destinatario);
                }

                if(correoCopia != null || !correoCopia.IsEmpty())
                {
                    string[] copias = SepararCadenaDeCorreos(correoCopia);
                    foreach (string cc in copias)
                    {
                        correo.CC.Add(cc);
                    }
                }

                correo.Subject = asunto;
                correo.Body = string.Format("<html>\r\n<body>\r\n<p>{0}</p>\r\n\r\n<h3>Datos de la nómina</h3>\r\n<table>\r\n<tr>\r\n<td>\r\n<p><b>Fecha de cálculo</b></p>\r\n</td>\r\n<td>\r\n <p><b>{1}</b></p>\r\n</td>\r\n</tr>\r\n<tr>\r\n <td>\r\n  <p><b>Status</b></p>\r\n</td>\r\n <td>\r\n<p><b>{2}</b></p>\r\n</td>\r\n</tr>\r\n  <tr>\r\n <td>\r\n <p><b>Cantidad de filas de la nómina</b></p>\r\n</td>\r\n<td>\r\n<p><b>{3}</b></p>\r\n </td>\r\n</tr>\r\n</table>\r\n\r\n<h3>Contenido de la nómina</h3>\r\n{4}\r\n </body>\r\n </html>",
                    mensaje,
                    fechaCalculo,
                    status.Equals("1") ? "Procesado" : "En proceso",
                    cantidadFilas,
                    tablaExcel
                );

                correo.IsBodyHtml = true;

                using (var context = new CalculosDeNominaEntities())
                {
                    var registro = context.CalculosDeNomina.FirstOrDefault(m => m.ID.ToString() == id);
                    if (registro.ArchivoNomina != null)
                    {
                        MemoryStream ms = new MemoryStream(registro.ArchivoNomina);

                        Attachment adjunto = new Attachment(ms, registro.NombreCalculo + ".xlsx", MediaTypeNames.Application.Octet);
                        correo.Attachments.Add(adjunto);
                    }
                }

                SmtpClient client = new SmtpClient();
                client.Send(correo);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error al enviar correo: " + e.Message);
                Console.WriteLine("Detalles: " + e.InnerException.Message);
                throw new Exception("Error al enviar correo: " + e.Message);
            }

            return RedirectToAction("DetalleNomina", new { id = int.Parse(id) });
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


        private string[] SepararCadenaDeCorreos(string correos)
        {
            string[] arrayCorreos = correos.Split(',');
            for(int i = 0; i < arrayCorreos.Length; i++)
            {
                arrayCorreos[i] = arrayCorreos[i].Trim();
            }

            return arrayCorreos;
        }


        [HttpPost]
        public ActionResult MostrarModalCorreo()
        {
            return Json(new { ok = true });
        }

        [HttpPost]
        public ActionResult CerrarModalCorreo()
        {
            return Json(new { ok = false });
        }


        public ActionResult VolverAlMenu()
        {
            return RedirectToAction("MainMenu", "MainMenu");
        }
    }
}