using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Request visual preparado para PUT /api/Docentes/{id}.
	/// </summary>
	public class ActualizarDocenteRequestUI
	{
		public string NombreCompleto { get; set; } = string.Empty;

		public string Identificacion { get; set; } = string.Empty;

		public string CorreoInstitucional { get; set; } = string.Empty;
	}
}
