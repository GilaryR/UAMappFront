using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Request temporal de creación
	/// de coordinadores.
	///
	/// Futuro endpoint:
	///
	/// POST /api/coordinadores
	/// </summary>
	public class CrearCoordinadorRequestUI
	{
		public string NombreCompleto { get; set; } = string.Empty;

		public string Cedula { get; set; } = string.Empty;

		public string CorreoInstitucional { get; set; } = string.Empty;

		public string Celular { get; set; } = string.Empty;

		public string Estado { get; set; } = string.Empty;
	}
}
