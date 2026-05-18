using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Request visual preparado para actualizar disponibilidad docente.
	///
	/// Endpoint:
	/// PUT /api/Docentes/{id}/disponibilidad
	/// </summary>
	public class ActualizarDisponibilidadDocenteRequestUI
	{
		public List<DisponibilidadDocenteItem> Disponibilidades { get; set; } = new();
	}
}
