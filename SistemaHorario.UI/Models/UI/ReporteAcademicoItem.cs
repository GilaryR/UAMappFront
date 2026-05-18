using System;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Representa un reporte académico dentro de la interfaz.
	/// 
	/// Por ahora se usa con datos temporales.
	/// Cuando backend esté listo, este modelo puede llenarse
	/// con la respuesta del endpoint de reportes.
	/// </summary>
	public class ReporteAcademicoItem
	{
		public int IdReporte { get; set; }

		public DateTime Fecha { get; set; }

		public string FechaTexto => Fecha.ToString("dd/MM/yyyy");

		public string TipoReporte { get; set; } = string.Empty;

		public string Usuario { get; set; } = string.Empty;

		public string Detalle { get; set; } = string.Empty;

		public string Periodo { get; set; } = string.Empty;

		public string FormatoInicial { get; set; } = string.Empty;

		public string Descripcion { get; set; } = string.Empty;
	}
}
