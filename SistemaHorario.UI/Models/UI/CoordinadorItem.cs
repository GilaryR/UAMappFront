using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Modelo visual de coordinador.
	///
	/// Actualmente trabaja con datos mock.
	///
	/// Más adelante representará la respuesta
	/// del endpoint:
	///
	/// GET /api/coordinadores
	/// </summary>
	public class CoordinadorItem
	{
		public int IdCoordinador { get; set; }

		public string NombreCompleto { get; set; } = string.Empty;

		public string Cedula { get; set; } = string.Empty;

		public string CorreoInstitucional { get; set; } = string.Empty;

		public string Rol { get; set; } = "Coordinador";

		public string Estado { get; set; } = "Activo";

		public string Celular { get; set; } = string.Empty;
	}
}
