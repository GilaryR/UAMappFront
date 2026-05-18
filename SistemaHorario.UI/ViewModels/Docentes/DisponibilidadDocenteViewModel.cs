using System;
using System.Text;
using System.Threading.Tasks;
using SistemaHorario.UI.Models.UI;
using System.Collections.Generic;
using System.Linq;

namespace SistemaHorario.UI.ViewModels.Docentes
{
	/// <summary>
	/// ViewModel encargado de administrar la disponibilidad visual
	/// de los docentes dentro de la interfaz del sistema.
	///
	/// Permite:
	/// - Mostrar las franjas horarias disponibles.
	/// - Modificar el estado de disponibilidad de una franja.
	/// - Gestionar la información utilizada por la vista.
	///
	/// TODO:
	/// Conectar con GET/PUT /api/Docentes/{id}/disponibilidad.
	/// </summary>
	public class DisponibilidadDocenteViewModel
	{
		/// <summary>
		/// Lista de disponibilidades del docente.
		///
		/// Cada elemento representa una franja horaria específica,
		/// indicando:
		/// - Día.
		/// - Hora de inicio.
		/// - Estado de disponibilidad.
		/// </summary>
		public List<DisponibilidadDocenteItem> Disponibilidades { get; set; }

		/// <summary>
		/// Constructor del ViewModel.
		///
		/// Inicializa la lista de disponibilidades recibida
		/// desde la capa de datos o desde un almacenamiento mock.
		/// </summary>
		/// <param name="disponibilidades">
		/// Lista de franjas horarias asociadas al docente.
		/// </param>
		public DisponibilidadDocenteViewModel(
			List<DisponibilidadDocenteItem> disponibilidades)
		{
			Disponibilidades = disponibilidades;
		}

		/// <summary>
		/// Cambia el estado de disponibilidad de una franja horaria.
		///
		/// Busca una coincidencia por:
		/// - Día.
		/// - Hora de inicio.
		///
		/// Si la franja existe:
		/// - Disponible = true  → pasa a false.
		/// - Disponible = false → pasa a true.
		///
		/// Si no se encuentra la franja, no realiza ninguna acción.
		/// </summary>
		/// <param name="dia">
		/// Día de la semana correspondiente a la franja.
		/// </param>
		/// <param name="hora">
		/// Hora de inicio de la franja horaria.
		/// </param>
		public void CambiarEstadoFranja(
			string dia,
			string hora)
		{
			DisponibilidadDocenteItem? franja =
				Disponibilidades.FirstOrDefault(f =>
					f.Dia == dia &&
					f.HoraInicio == hora);

			if (franja == null)
				return;

			franja.Disponible = !franja.Disponible;
		}
	}
}