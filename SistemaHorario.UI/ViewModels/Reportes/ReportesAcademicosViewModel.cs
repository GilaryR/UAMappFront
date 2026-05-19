using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
		private readonly ReportesApiService _api = new();
		private List<ReporteAcademicoItem> _reportesBase = new();

		public List<ReporteAcademicoItem> Reportes { get; private set; } = new();

		public List<string> TiposReporte { get; private set; } = new();

		public List<string> Periodos { get; private set; } = new();

		public string MensajeEstado { get; private set; } = string.Empty;

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

		public async Task CargarCatalogosAsync()
		{
			var tiposResp = await _api.ObtenerTiposReporteAsync();
			if (tiposResp.Success && tiposResp.Data != null && tiposResp.Data.Count > 0)
			{
				TiposReporte = tiposResp.Data;
			}
			else
			{
				MensajeEstado = tiposResp.Message;
			}

			var periodosResp = await _api.ObtenerSemestresAsync();
			if (periodosResp.Success && periodosResp.Data != null && periodosResp.Data.Count > 0)
			{
				Periodos = periodosResp.Data.Select(x => x.ToString()).ToList();
			}
			else if (string.IsNullOrWhiteSpace(MensajeEstado))
			{
				MensajeEstado = periodosResp.Message;
			}
		}

		public async Task CargarReportesAsync()
		{
			var reportes = new List<ReporteAcademicoItem>();

			var generalResp = await _api.ObtenerReporteGeneralAsync();
			if (generalResp.Success && generalResp.Data != null)
			{
				reportes.Add(new ReporteAcademicoItem
				{
					IdReporte = 1,
					Fecha = DateTime.Now,
					TipoReporte = "Reporte general",
					Usuario = "Sistema",
					Detalle = "Resumen de métricas generales",
					Periodo = "Global",
					FormatoInicial = "PDF",
					Descripcion = $"Usuarios: {generalResp.Data.TotalUsuarios}, Materias: {generalResp.Data.TotalMaterias}, Planes: {generalResp.Data.TotalPlanesAcademicos}"
				});
			}

			var usuariosRolResp = await _api.ObtenerUsuariosPorRolAsync();
			if (usuariosRolResp.Success && usuariosRolResp.Data != null)
			{
				reportes.AddRange(usuariosRolResp.Data.Select((item, index) => new ReporteAcademicoItem
				{
					IdReporte = 10 + index,
					Fecha = DateTime.Now,
					TipoReporte = "Usuarios por rol",
					Usuario = "Sistema",
					Detalle = item.Rol,
					Periodo = "Global",
					FormatoInicial = "CSV",
					Descripcion = $"Usuarios totales para el rol {item.Rol}: {item.TotalUsuarios}"
				}));
			}

			var materiasResp = await _api.ObtenerMateriasPorSemestreAsync();
			if (materiasResp.Success && materiasResp.Data != null)
			{
				reportes.AddRange(materiasResp.Data.Select(item => new ReporteAcademicoItem
				{
					IdReporte = 20 + item.Semestre,
					Fecha = DateTime.Now,
					TipoReporte = "Materias por semestre",
					Usuario = "Sistema",
					Detalle = $"Semestre {item.Semestre}",
					Periodo = item.Semestre.ToString(),
					FormatoInicial = "CSV",
					Descripcion = $"Total materias: {item.TotalMaterias}."
				}));
			}

			var franjasResp = await _api.ObtenerFranjasPorDiaAsync();
			if (franjasResp.Success && franjasResp.Data != null)
			{
				reportes.AddRange(franjasResp.Data.Select((item, index) => new ReporteAcademicoItem
				{
					IdReporte = 40 + index,
					Fecha = DateTime.Now,
					TipoReporte = "Franjas por día",
					Usuario = "Sistema",
					Detalle = item.Dia,
					Periodo = "Global",
					FormatoInicial = "PDF",
					Descripcion = $"Total franjas: {item.TotalFranjas}."
				}));
			}

			var planesResp = await _api.ObtenerPlanesAcademicosAsync();
			if (planesResp.Success && planesResp.Data != null)
			{
				reportes.AddRange(planesResp.Data.Select(item => new ReporteAcademicoItem
				{
					IdReporte = 60 + item.IdPlanAcademico,
					Fecha = DateTime.Now,
					TipoReporte = "Planes académicos",
					Usuario = "Sistema",
					Detalle = item.Nombre,
					Periodo = item.Anio.ToString(),
					FormatoInicial = "PDF",
					Descripcion = $"Programa: {item.Programa}, Materias: {item.TotalMaterias}."
				}));
			}

			if (reportes.Count > 0)
			{
				_reportesBase = reportes.OrderByDescending(r => r.Fecha).ToList();
				Reportes = _reportesBase.ToList();
				return;
			}

			MensajeEstado = string.IsNullOrWhiteSpace(MensajeEstado)
				? "No se encontraron reportes desde el backend. Se usan datos locales."
				: MensajeEstado;

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
			if (reporte.IdReporte == 0)
				reporte.IdReporte = _reportesBase.Count > 0
					? _reportesBase.Max(r => r.IdReporte) + 1
					: 1;

			if (string.IsNullOrWhiteSpace(reporte.Usuario))
				reporte.Usuario = "admin";

			if (string.IsNullOrWhiteSpace(reporte.Detalle))
				reporte.Detalle = "-";

			_reportesBase.Insert(0, reporte);
			AplicarFiltros();
		}
		
	}
}
