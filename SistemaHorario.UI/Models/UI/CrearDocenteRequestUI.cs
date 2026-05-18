using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Request visual preparado para POST /api/Docentes.
	///
	/// Contrato actual backend:
	/// {
	///   "nombreCompleto": "",
	///   "identificacion": "",
	///   "correoInstitucional": ""
	/// }
	/// </summary>
	public class CrearDocenteRequestUI
	{
		public string NombreCompleto { get; set; } = string.Empty;

		public string Identificacion { get; set; } = string.Empty;

		public string CorreoInstitucional { get; set; } = string.Empty;
	}
}
