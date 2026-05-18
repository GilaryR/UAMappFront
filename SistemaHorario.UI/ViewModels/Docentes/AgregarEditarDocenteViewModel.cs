using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaHorario.UI.Models.UI;
using System.Collections.Generic;

namespace SistemaHorario.UI.ViewModels.Docentes
{
	/// <summary>
	/// ViewModel encargado de manejar la información necesaria
	/// para agregar, editar o visualizar un docente dentro de la interfaz.
	///
	/// Esta clase contiene:
	/// - Los datos principales del docente.
	/// - La disponibilidad horaria del docente.
	/// - Un indicador para saber si el formulario está en modo edición.
	///
	/// TODO:
	/// Conectar creación y edición con POST/PUT /api/Docentes.
	/// </summary>
	public class AgregarEditarDocenteViewModel
	{
		/// <summary>
		/// Objeto que almacena la información principal del docente,
		/// como nombre, identificación, correo institucional,
		/// materias asignadas y estado.
		/// </summary>
		public DocenteItem Docente { get; set; }

		/// <summary>
		/// Lista que representa la disponibilidad horaria del docente.
		/// Cada elemento indica un bloque de disponibilidad asociado
		/// a un día y horario específico.
		/// </summary>
		public List<DisponibilidadDocenteItem> Disponibilidad { get; set; }

		/// <summary>
		/// Indica si el formulario se encuentra en modo edición.
		///
		/// Valor:
		/// - true: se está editando un docente existente.
		/// - false: se está creando un nuevo docente.
		/// </summary>
		public bool EsEdicion { get; set; }

		/// <summary>
		/// Constructor utilizado cuando se va a crear un nuevo docente.
		///
		/// Inicializa un docente vacío con estado "Activo",
		/// carga una disponibilidad base desde el almacenamiento mock
		/// y establece el formulario en modo creación.
		/// </summary>
		public AgregarEditarDocenteViewModel()
		{
			Docente = new DocenteItem
			{
				Estado = "Activo"
			};

			Disponibilidad = DocentesMockStore.CrearDisponibilidadBase();

			EsEdicion = false;
		}

		/// <summary>
		/// Constructor utilizado cuando se va a editar un docente existente.
		///
		/// Recibe un objeto DocenteItem con la información actual del docente,
		/// copia sus datos principales a una nueva instancia y carga
		/// la disponibilidad correspondiente a ese docente desde el mock store.
		/// </summary>
		/// <param name="docente">
		/// Docente existente que será cargado en el formulario para edición.
		/// </param>
		public AgregarEditarDocenteViewModel(DocenteItem docente)
		{
			Docente = new DocenteItem
			{
				IdDocente = docente.IdDocente,
				NombreCompleto = docente.NombreCompleto,
				Identificacion = docente.Identificacion,
				CorreoInstitucional = docente.CorreoInstitucional,
				Materias = docente.Materias,
				Estado = docente.Estado
			};

			Disponibilidad =
				DocentesMockStore.ObtenerDisponibilidad(docente.IdDocente);

			EsEdicion = true;
		}
	}
}