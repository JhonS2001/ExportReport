namespace ExportReport.Models
{
public class ReporteData
{
        // Datos importantes del reporte
        public string NombreArchivo { get; set; }
        public string TituloReporte { get; set; }
        public string VCC { get; set; }
        public string FechaGeneracion { get; set; }
        public string Periodo { get; set; }

        //Diseño de la página
        public string TamañoPagina { get; set; }
        public float MargenIzquierdo { get; set; }
        public float MargenDerecho { get; set; }
        public float MargenSuperior { get; set; }
        public float MargenInferior { get; set; }
        public string FuenteTitulo { get; set; }
        public string FuenteCeldasEncabezado { get; set; }
        public string FuenteCeldasContenido { get; set; }
        public float TamañoLetraTitulo { get; set; }
        public float TamañoLetraCeldasEncabezado { get; set; }
        public float TamañoLetraCeldasContenido { get; set; }

        //Datos de la tabla
        public List<DatosTabla> DatosTabla { get; set; }
    }
}
