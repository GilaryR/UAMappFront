using SistemaHorario.UI.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaHorario.UI.ViewModels.Reportes
{
	/// <summary>
	/// Maneja la información que usa la vista de reportes.
	/// 
	/// Aquí se cargan reportes, tipos de reporte, periodos y filtros.
	/// Cuando se conecte backend, se reemplaza el MockStore por servicios.
	/// </summary>
	public class ReportesAcademicosViewModel
	{
		private List<ReporteAcademicoItem> _reportesBase = new();

		public List<ReporteAcademicoItem> Reportes { get; private set; } = new();

		public List<string> TiposReporte { get; private set; } = new();

		public List<string> Periodos { get; private set; } = new();

		public string Busqueda { get; set; } = string.Empty;

		public string TipoSeleccionado { get; set; } = "Todos los tipos";

		public DateTime? FechaSeleccionada { get; set; }

		public ReportesAcademicosViewModel()
		{
			CargarDatos();
		}

		public void CargarDatos()
		{
			TiposReporte = ReportesAcademicosMockStore.ObtenerTiposReporte();
			Periodos = ReportesAcademicosMockStore.ObtenerPeriodos();

			_reportesBase = ReportesAcademicosMockStore.ObtenerReportes();
			Reportes = _reportesBase.ToList();
		}

		public void AplicarFiltros()
		{
			IEnumerable<ReporteAcademicoItem> consulta = _reportesBase;

			if (!string.IsNullOrWhiteSpace(Busqueda))
			{
				string busqueda = Busqueda.Trim().ToLower();

				consulta = consulta.Where(reporte =>
					reporte.TipoReporte.ToLower().Contains(busqueda) ||
					reporte.Usuario.ToLower().Contains(busqueda) ||
					reporte.Detalle.ToLower().Contains(busqueda) ||
					reporte.Periodo.ToLower().Contains(busqueda));
			}

			if (!string.IsNullOrWhiteSpace(TipoSeleccionado) &&
				TipoSeleccionado != "Todos los tipos")
			{
				consulta = consulta.Where(reporte =>
					reporte.TipoReporte == TipoSeleccionado);
			}

			if (FechaSeleccionada.HasValue)
			{
				consulta = consulta.Where(reporte =>
					reporte.Fecha.Date == FechaSeleccionada.Value.Date);
			}

			Reportes = consulta.ToList();
		}

		public void LimpiarFiltros()
		{
			Busqueda = string.Empty;
			TipoSeleccionado = "Todos los tipos";
			FechaSeleccionada = null;

			Reportes = _reportesBase.ToList();
		}

		public void AgregarReporte(ReporteAcademicoItem reporte)
		{
			ReportesAcademicosMockStore.AgregarReporte(reporte);

			_reportesBase = ReportesAcademicosMockStore.ObtenerReportes();
			AplicarFiltros();
		}
	}
}
