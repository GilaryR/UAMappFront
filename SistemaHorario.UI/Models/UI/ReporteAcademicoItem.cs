using System;

namespace SistemaHorario.UI.Models.UI
{
    // Representa una opción de reporte visible en la tabla del módulo.
    public class ReporteAcademicoItem
    {
        public int IdReporte { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string FechaTexto => Fecha.ToString("dd/MM/yyyy");

        public string TipoCodigo { get; set; } = string.Empty;

        public string TipoReporte { get; set; } = string.Empty;

        public string Usuario { get; set; } = "Sistema";

        public string Detalle { get; set; } = string.Empty;

        public string Periodo { get; set; } = string.Empty;

        public string FormatoInicial { get; set; } = "CSV";

        public string Descripcion { get; set; } = string.Empty;

        public int? IdGrupo { get; set; }

        public int? IdDocente { get; set; }

        public string Estado { get; set; } = string.Empty;
    }
}
