using SistemaHorario.UI.Models.UI;
using System.Collections.Generic;
using System.Linq;

namespace SistemaHorario.UI.ViewModels.Coordinadores
{
	/// <summary>
	/// ViewModel principal del módulo coordinadores.
	///
	/// Maneja:
	/// - carga temporal,
	/// - filtros,
	/// - búsqueda,
	/// - resumen visual.
	///
	/// Más adelante consumirá:
	///
	/// GET /api/coordinadores
	/// </summary>
	public class CoordinadoresViewModel
	{
		private List<CoordinadorItem> _coordinadoresBase = new();

		public List<CoordinadorItem> Coordinadores { get; private set; } = new();

		public ResumenCoordinadorItem Resumen { get; private set; } = new();

		/// <summary>
		/// Constructor principal.
		/// </summary>
		public CoordinadoresViewModel()
		{
			CargarDatos();
		}

		/// <summary>
		/// Carga datos mock.
		/// </summary>
		public void CargarDatos()
		{
			_coordinadoresBase =
				CoordinadoresMockStore.Obtener();

			Coordinadores =
				_coordinadoresBase.ToList();

			CalcularResumen();
		}

		/// <summary>
		/// Filtra coordinadores.
		/// </summary>
		public void Filtrar(
			string textoBusqueda,
			string filtroBusqueda,
			string estado)
		{
			IEnumerable<CoordinadorItem> query =
				_coordinadoresBase;

			if (!string.IsNullOrWhiteSpace(textoBusqueda))
			{
				textoBusqueda =
					textoBusqueda.ToLower();

				switch (filtroBusqueda.ToLower())
				{
					case "nombre":
						query = query.Where(x =>
							x.NombreCompleto
								.ToLower()
								.Contains(textoBusqueda));
						break;

					case "cedula":
						query = query.Where(x =>
							x.Cedula
								.ToLower()
								.Contains(textoBusqueda));
						break;

					case "correo":
						query = query.Where(x =>
							x.CorreoInstitucional
								.ToLower()
								.Contains(textoBusqueda));
						break;
				}
			}

			if (estado != "Todos")
			{
				query = query.Where(x =>
					x.Estado == estado);
			}

			Coordinadores = query.ToList();

			CalcularResumen();
		}

		/// <summary>
		/// Calcula resumen visual.
		/// </summary>
		private void CalcularResumen()
		{
			Resumen = new ResumenCoordinadorItem
			{
				TotalCoordinadores = Coordinadores.Count,

				Activos = Coordinadores.Count(x =>
					x.Estado == "Activo"),

				Inactivos = Coordinadores.Count(x =>
					x.Estado == "Inactivo")
			};
		}
	}
}