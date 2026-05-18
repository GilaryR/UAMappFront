using SistemaHorario.UI.Models.UI;
using System.Collections.Generic;
using System.Linq;

namespace SistemaHorario.UI.ViewModels.Docentes
{
	/// <summary>
	/// ViewModel principal del módulo Docentes.
	///
	/// Se encarga de administrar la información que se muestra
	/// en la vista principal de docentes.
	///
	/// Maneja:
	/// - Carga temporal de docentes.
	/// - Búsqueda por nombre, identificación o correo.
	/// - Filtro por estado.
	/// - Cálculo del resumen general del módulo.
	///
	/// TODO:
	/// Reemplazar DocentesMockStore por GET /api/Docentes.
	/// </summary>
	public class DocentesViewModel
	{
		/// <summary>
		/// Lista base de docentes cargada desde el almacenamiento temporal.
		///
		/// Esta lista conserva todos los docentes originales y se utiliza
		/// como fuente para aplicar búsquedas y filtros sin perder datos.
		/// </summary>
		private List<DocenteItem> _docentesBase = new();

		/// <summary>
		/// Lista de docentes que se muestra actualmente en la interfaz.
		///
		/// Puede contener:
		/// - Todos los docentes cargados.
		/// - Solo los docentes filtrados por búsqueda.
		/// - Solo los docentes filtrados por estado.
		/// </summary>
		public List<DocenteItem> Docentes { get; private set; } = new();

		/// <summary>
		/// Objeto que contiene el resumen estadístico del módulo Docentes.
		///
		/// Incluye totales como:
		/// - Total de docentes.
		/// - Docentes activos.
		/// - Docentes inactivos.
		/// - Docentes disponibles.
		/// </summary>
		public ResumenDocenteItem Resumen { get; private set; } = new();

		/// <summary>
		/// Constructor del ViewModel.
		///
		/// Al crear una instancia, carga automáticamente los datos
		/// iniciales de docentes y calcula el resumen.
		/// </summary>
		public DocentesViewModel()
		{
			CargarDatos();
		}

		/// <summary>
		/// Carga los datos de docentes desde el almacenamiento temporal.
		///
		/// Obtiene la lista completa desde DocentesMockStore,
		/// la copia a la lista visible y luego recalcula el resumen.
		/// </summary>
		public void CargarDatos()
		{
			_docentesBase = DocentesMockStore.ObtenerDocentes();
			Docentes = _docentesBase.ToList();

			CalcularResumen();
		}

		/// <summary>
		/// Aplica filtros sobre la lista base de docentes.
		///
		/// Permite filtrar por:
		/// - Texto de búsqueda.
		/// - Estado del docente.
		///
		/// El texto de búsqueda se compara contra:
		/// - Nombre completo.
		/// - Identificación.
		/// - Correo institucional.
		///
		/// Luego actualiza la lista visible y recalcula el resumen.
		/// </summary>
		/// <param name="textoBusqueda">
		/// Texto ingresado por el usuario para buscar docentes.
		/// </param>
		/// <param name="estado">
		/// Estado seleccionado para filtrar.
		/// Puede ser "Todos", "Activo" o "Inactivo".
		/// </param>
		public void Filtrar(
			string textoBusqueda,
			string estado)
		{
			IEnumerable<DocenteItem> consulta = _docentesBase;

			if (!string.IsNullOrWhiteSpace(textoBusqueda))
			{
				string texto = textoBusqueda.Trim().ToLower();

				consulta = consulta.Where(d =>
					d.NombreCompleto.ToLower().Contains(texto) ||
					d.Identificacion.ToLower().Contains(texto) ||
					d.CorreoInstitucional.ToLower().Contains(texto));
			}

			if (!string.IsNullOrWhiteSpace(estado) &&
				estado != "Todos")
			{
				consulta = consulta.Where(d =>
					d.Estado == estado);
			}

			Docentes = consulta.ToList();

			CalcularResumen();
		}

		/// <summary>
		/// Calcula el resumen estadístico de la lista visible de docentes.
		///
		/// El resumen se basa en los docentes actualmente mostrados,
		/// es decir, después de aplicar búsquedas o filtros.
		///
		/// Calcula:
		/// - Total de docentes.
		/// - Cantidad de docentes activos.
		/// - Cantidad de docentes inactivos.
		/// - Cantidad de docentes disponibles.
		/// </summary>
		private void CalcularResumen()
		{
			Resumen = new ResumenDocenteItem
			{
				TotalDocentes = Docentes.Count,
				Activos = Docentes.Count(d => d.Estado == "Activo"),
				Inactivos = Docentes.Count(d => d.Estado == "Inactivo"),
				Disponibles = Docentes.Count
			};
		}
	}
}