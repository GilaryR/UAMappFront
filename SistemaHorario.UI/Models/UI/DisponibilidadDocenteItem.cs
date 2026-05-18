using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Modelo visual para una franja de disponibilidad docente.
	///
	/// Preparado para:
	/// GET /api/Docentes/{id}/disponibilidad
	/// PUT /api/Docentes/{id}/disponibilidad
	/// </summary>
	public class DisponibilidadDocenteItem
	{
		public string Dia { get; set; } = string.Empty;

		public string HoraInicio { get; set; } = string.Empty;

		public string HoraFin { get; set; } = string.Empty;

		public bool Disponible { get; set; }
	}
}
