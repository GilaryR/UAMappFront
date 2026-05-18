using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Datos temporales para el módulo de Reportes Académicos.
	/// 
	/// Este archivo concentra la información quemada para que después
	/// el compañero de backend la reemplace por endpoints.
	/// </summary>
	public static class ReportesAcademicosMockStore
	{
		private static readonly List<ReporteAcademicoItem> _reportes = new();

		private static int _siguienteId = 20;

		public static List<string> ObtenerTiposReporte()
		{
			return new List<string>
			{
				"Carga Docente",
				"Horarios Aprobado",
				"Estudiantes Tapsi"
			};
		}

		public static List<string> ObtenerPeriodos()
		{
			return new List<string>
			{
				"2026-1",
				"2026-2",
				"2027-1"
			};
		}

		public static List<ReporteAcademicoItem> ObtenerReportes()
		{
			InicializarSiEstaVacio();

			return _reportes
				.OrderByDescending(reporte => reporte.Fecha)
				.ToList();
		}

		public static void AgregarReporte(ReporteAcademicoItem reporte)
		{
			reporte.IdReporte = _siguienteId;
			_siguienteId++;

			if (string.IsNullOrWhiteSpace(reporte.Usuario))
				reporte.Usuario = "admin";

			if (string.IsNullOrWhiteSpace(reporte.Detalle))
				reporte.Detalle = "Diurna";

			_reportes.Insert(0, reporte);
		}

		private static void InicializarSiEstaVacio()
		{
			if (_reportes.Count > 0)
				return;

			_reportes.Add(new ReporteAcademicoItem
			{
				IdReporte = 1,
				Fecha = new DateTime(2026, 4, 27),
				TipoReporte = "Carga Docente",
				Usuario = "admin",
				Detalle = "Diurna",
				Periodo = "2026-1",
				FormatoInicial = "PDF",
				Descripcion = "Reporte de carga docente por jornada."
			});

			_reportes.Add(new ReporteAcademicoItem
			{
				IdReporte = 2,
				Fecha = new DateTime(2026, 4, 27),
				TipoReporte = "Horarios Aprobado",
				Usuario = "coordinador",
				Detalle = "Diurna",
				Periodo = "2026-1",
				FormatoInicial = "CSV",
				Descripcion = "Reporte de horarios aprobados."
			});

			_reportes.Add(new ReporteAcademicoItem
			{
				IdReporte = 3,
				Fecha = new DateTime(2026, 4, 27),
				TipoReporte = "Estudiantes Tapsi",
				Usuario = "admin",
				Detalle = "Nocturna",
				Periodo = "2026-1",
				FormatoInicial = "PDF",
				Descripcion = "Reporte de estudiantes TAPSI."
			});
		}
	}
}
