using ExportReport.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Document = iTextSharp.text.Document;

[Route("api/[controller]")]
[ApiController]
public class PdfController : ControllerBase
{
    [HttpPost]
    public IActionResult GeneratePdf([FromBody] ReporteData reporteData)
    {
        // Validar que se env�e al menos un dato en datosTabla.
        if (reporteData.DatosTabla == null || reporteData.DatosTabla.Count == 0)
        {
            return BadRequest(new ErrorResult
            {
                Status = false,
                StatusCode = 400,
                Descripcion = "Datos inv�lidos",
                Corregir = "Debe incluir al menos un dato en la propiedad datosTabla."
            });
        }

        try
        {
            // Validar si los datos requeridos est�n presentes.
            if (string.IsNullOrWhiteSpace(reporteData.NombreArchivo) ||
                string.IsNullOrWhiteSpace(reporteData.TituloReporte) ||
                string.IsNullOrWhiteSpace(reporteData.VCC) ||
                string.IsNullOrWhiteSpace(reporteData.FechaGeneracion) ||
                string.IsNullOrWhiteSpace(reporteData.Periodo) ||
                reporteData.DatosTabla == null || !reporteData.DatosTabla.Any())
            {
                return BadRequest(new ErrorResult
                {
                    Status = false,
                    StatusCode = 400,
                    Descripcion = "Datos inv�lidos o incompletos",
                    Corregir = "Los datos proporcionados son inv�lidos o incompletos."
                });
            }

            // Validar si todos los datos de la tabla tienen la misma cantidad de filas.
            int totalFilas = reporteData.DatosTabla.First().DatosColumna.Count;
            if (reporteData.DatosTabla.Any(d => d.DatosColumna.Count != totalFilas))
            {
                return BadRequest(new ErrorResult
                {
                    Status = false,
                    StatusCode = 400,
                    Descripcion = "Faltan datos o el formato es incorrecto.",
                    Corregir = "Aseg�rate de proporcionar todos los datos necesarios y seguir el formato correcto del JSON."
                });
            }

            // Datos adicionales.
            string nombreArchivo = reporteData.NombreArchivo;
            string tituloReporte = reporteData.TituloReporte;
            string vcc = reporteData.VCC;
            string fechaGeneracion = reporteData.FechaGeneracion;
            string periodo = reporteData.Periodo;

            // Opciones de formato del documento PDF
            string tama�oPagina = reporteData.Tama�oPagina;
            float margenIzquierdo = reporteData.MargenIzquierdo;
            float margenDerecho = reporteData.MargenDerecho;
            float margenSuperior = reporteData.MargenSuperior;
            float margenInferior = reporteData.MargenInferior;

            // Fuentes y tama�os de letra
            string fuenteTitulo = reporteData.FuenteTitulo;
            string fuenteCeldasEncabezado = reporteData.FuenteCeldasEncabezado;
            string fuenteCeldasContenido = reporteData.FuenteCeldasContenido;
            float tama�oLetraTitulo = reporteData.Tama�oLetraTitulo;
            float tama�oLetraCeldasEncabezado = reporteData.Tama�oLetraCeldasEncabezado;
            float tama�oLetraCeldasContenido = reporteData.Tama�oLetraCeldasContenido;

            // Crea un nuevo documento PDF con el tama�o de p�gina y m�rgenes especificados.
            Document document = new Document(PageSizePDF.GetPageSize(tama�oPagina), margenIzquierdo, margenDerecho, margenSuperior, margenInferior);

            // Crea un nuevo documento PDF.
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            // Abre el documento para escribir.
            document.Open();

            // Estilos para el reporte.
            Font fontTitulo = FontFactory.GetFont(fuenteTitulo, tama�oLetraTitulo, BaseColor.BLACK);
            Font fontCeldasEncabezado = FontFactory.GetFont(fuenteCeldasEncabezado, tama�oLetraCeldasEncabezado, BaseColor.WHITE); // Encabezados con fondo blanco
            Font fontCeldasContenido = FontFactory.GetFont(fuenteCeldasContenido, tama�oLetraCeldasContenido, BaseColor.BLACK);

            // Agrega el contenido al PDF.
            // T�tulo del reporte
            Paragraph tituloParagraph = new Paragraph(tituloReporte.ToUpper(), fontTitulo);
            tituloParagraph.Alignment = Element.ALIGN_CENTER;
            document.Add(tituloParagraph);

            // Datos adicionales
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("VCC:                            " + vcc, fontCeldasContenido));
            document.Add(new Paragraph("Fecha de generaci�n: " + fechaGeneracion, fontCeldasContenido));
            document.Add(new Paragraph("Per�odo:                      " + periodo, fontCeldasContenido));
            document.Add(new Paragraph("\n"));

            // Crea la tabla y establece el n�mero de columnas
            int totalColumnas = reporteData.DatosTabla.Count;
            PdfPTable table = new PdfPTable(totalColumnas);
            table.WidthPercentage = 100;

            // Agrega los encabezados de la tabla
            foreach (var dato in reporteData.DatosTabla)
            {
                PdfPCell cell = new PdfPCell(new Phrase(dato.NameEncabezado.ToUpper(), fontCeldasEncabezado));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BackgroundColor = new BaseColor(255, 121, 8); // Fondo blanco para los encabezados
                cell.PaddingTop = 4; // Padding superior de la celda
                cell.PaddingBottom = 4; // Padding inferior de la celda
                cell.BorderColor = BaseColor.WHITE; // Borde blanco
                table.AddCell(cell);
            }

            // Agrega los datos de la tabla al PDF.
            int totalFilas2 = reporteData.DatosTabla.First().DatosColumna.Count; // Se asume que todas las columnas tienen la misma cantidad de datos.
            for (int i = 0; i < totalFilas2; i++)
            {
                foreach (var dato in reporteData.DatosTabla)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(dato.DatosColumna[i], fontCeldasContenido));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.WHITE; // Borde blanco
                    cell.PaddingTop = 4; // Padding superior de la celda
                    cell.PaddingBottom = 4; // Padding inferior de la celda
                    table.AddCell(cell);
                }
            }

            // Agrega la tabla al documento
            document.Add(table);

            // Cierra el documento.
            document.Close();

            // Descarga el PDF como un archivo.
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            
            // Convierte el arreglo de bytes a Base64.
            string base64String = Convert.ToBase64String(bytes);

            // Retorna el resultado en Base64.
            return Ok(new { Base64Content = base64String });
        }
        catch (Exception ex)
        {
            // En caso de una excepci�n, devolvemos una respuesta de error con detalles.
            return StatusCode(500, new ErrorResult
            {
                Status = false,
                StatusCode = 500,
                Descripcion = "Ocurri� un error al generar el PDF.",
                Corregir = "Revise los datos proporcionados y aseg�rese de que sean v�lidos y correctos."
            });
        }
    }

    public class ErrorResult
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string? Descripcion { get; set; }
        public string? Corregir { get; set; }
    }
}
