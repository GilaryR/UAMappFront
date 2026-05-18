using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Almacén temporal para validar la interfaz del módulo
	/// Historial de cambios sin conexión a backend.
	///
	/// TODO:
	/// Reemplazar esta clase por consumo real de endpoint.
	///
	/// Endpoint sugerido:
	/// GET /api/historial-cambios
	///
	/// Parámetros sugeridos:
	/// - usuario
	/// - modulo
	/// - fechaDesde
	/// - fechaHasta
	///
	/// Ejemplo:
	/// GET /api/historial-cambios?usuario=Coordinador&modulo=Horarios&fechaDesde=2026-04-27&fechaHasta=2026-04-30
	/// </summary>
	public static class HistorialCambiosMockStore
	{
		private static readonly List<HistorialCambioItem> _historial =
		[
			new HistorialCambioItem
			{
				IdHistorial = 1,
				FechaHora = new DateTime(2026, 4, 27, 7, 26, 0),
				Usuario = "Coordinador",
				Modulo = "Horarios",
				Descripcion = "actualizó límite de créditos nocturno para validación."
			},

			new HistorialCambioItem
			{
				IdHistorial = 2,
				FechaHora = new DateTime(2026, 4, 27, 10, 45, 0),
				Usuario = "Coordinador",
				Modulo = "Horarios",
				Descripcion = "generó propuesta PROP-2026-01-A."
			},

			new HistorialCambioItem
			{
				IdHistorial = 3,
				FechaHora = new DateTime(2026, 4, 27, 15, 40, 0),
				Usuario = "Coordinador",
				Modulo = "Docentes",
				Descripcion = "registró disponibilidad de Marcela."
			},

			new HistorialCambioItem
			{
				IdHistorial = 4,
				FechaHora = new DateTime(2026, 4, 27, 17, 30, 0),
				Usuario = "Coordinador",
				Modulo = "Materias",
				Descripcion = "configuró materias obligatorias TAPSI."
			},

			new HistorialCambioItem
			{
				IdHistorial = 5,
				FechaHora = new DateTime(2026, 4, 28, 9, 15, 0),
				Usuario = "Coordinador",
				Modulo = "Plan académico",
				Descripcion = "actualizó prerrequisitos del plan académico."
			},

			new HistorialCambioItem
			{
				IdHistorial = 6,
				FechaHora = new DateTime(2026, 4, 29, 11, 10, 0),
				Usuario = "Coordinador",
				Modulo = "Reportes",
				Descripcion = "generó reporte de disponibilidad docente."
			},

			new HistorialCambioItem
			{
				IdHistorial = 7,
				FechaHora = new DateTime(2026, 4, 30, 14, 0, 0),
				Usuario = "Coordinador",
				Modulo = "Grupos académicos",
				Descripcion = "editó la información del grupo académico G1."
			}
		];

		/// <summary>
		/// Retorna todos los registros temporales del historial.
		/// </summary>
		public static List<HistorialCambioItem> ObtenerHistorial()
		{
			return _historial
				.OrderByDescending(h => h.FechaHora)
				.ToList();
		}
	}
}