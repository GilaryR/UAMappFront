using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Modelo visual utilizado en la tabla principal de docentes.
	///
	/// Se prepara para mapear datos desde GET /api/Docentes.
	///
	/// Nota:
	/// El backend actual solo maneja nombreCompleto,
	/// identificacion y correoInstitucional.
	/// Materias y Estado quedan temporales para UI.
	/// </summary>
	public class DocenteItem
	{
		public int IdDocente { get; set; }

		public string NombreCompleto { get; set; } = string.Empty;

		public string Identificacion { get; set; } = string.Empty;

		public string CorreoInstitucional { get; set; } = string.Empty;

		public string Materias { get; set; } = string.Empty;

		public string Estado { get; set; } = "Activo";
	}
}
