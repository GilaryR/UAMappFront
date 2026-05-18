using SistemaHorario.UI.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaHorario.UI.ViewModels.HistorialCambios
{
	/// <summary>
	/// ViewModel principal del módulo Historial de cambios.
	///
	/// Maneja:
	/// - carga temporal de actividades,
	/// - filtro por usuario,
	/// - filtro por módulo,
	/// - filtro por fecha única o rango de fechas.
	///
	/// TODO:
	/// Reemplazar HistorialCambiosMockStore por consumo real:
	///
	/// GET /api/historial-cambios
	///
	/// Parámetros recomendados:
	/// - usuario
	/// - modulo
	/// - fechaDesde
	/// - fechaHasta
	/// </summary>
	public class HistorialCambiosViewModel
	{
		private List<HistorialCambioItem> _historialBase = new();

		/// <summary>
		/// Lista visual filtrada.
		/// </summary>
		public List<HistorialCambioItem> Actividades { get; private set; } = new();

		/// <summary>
		/// Constructor principal.
		/// </summary>
		public HistorialCambiosViewModel()
		{
			CargarDatos();
		}

		/// <summary>
		/// Carga datos temporales.
		/// </summary>
		public void CargarDatos()
		{
			_historialBase = HistorialCambiosMockStore.ObtenerHistorial();

			Actividades = _historialBase.ToList();
		}

		/// <summary>
		/// Filtra el historial por usuario, módulo y fechas.
		/// </summary>
		public void Filtrar(
			string usuario,
			string modulo,
			DateTime? fechaDesde,
			DateTime? fechaHasta)
		{
			IEnumerable<HistorialCambioItem> consulta = _historialBase;

			if (!string.IsNullOrWhiteSpace(usuario) &&
				usuario != "Todos")
			{
				consulta = consulta.Where(h =>
					h.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase));
			}

			if (!string.IsNullOrWhiteSpace(modulo) &&
				modulo != "Todos")
			{
				consulta = consulta.Where(h =>
					h.Modulo.Equals(modulo, StringComparison.OrdinalIgnoreCase));
			}

			if (fechaDesde.HasValue && !fechaHasta.HasValue)
			{
				DateTime fecha = fechaDesde.Value.Date;

				consulta = consulta.Where(h =>
					h.FechaHora.Date == fecha);
			}

			if (fechaDesde.HasValue && fechaHasta.HasValue)
			{
				DateTime desde = fechaDesde.Value.Date;
				DateTime hasta = fechaHasta.Value.Date;

				consulta = consulta.Where(h =>
					h.FechaHora.Date >= desde &&
					h.FechaHora.Date <= hasta);
			}

			Actividades = consulta
				.OrderByDescending(h => h.FechaHora)
				.ToList();
		}

		/// <summary>
		/// Limpia todos los filtros y restaura la lista original.
		/// </summary>
		public void LimpiarFiltros()
		{
			Actividades = _historialBase
				.OrderByDescending(h => h.FechaHora)
				.ToList();
		}
	}
}