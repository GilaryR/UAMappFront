using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Request temporal utilizado
	/// para validar credenciales antes
	/// de permitir crear coordinadores.
	///
	/// Futuro endpoint:
	///
	/// POST /api/auth/verificar-credenciales
	/// </summary>
	public class VerificarCredencialesRequestUI
	{
		public string Correo { get; set; } = string.Empty;

		public string Password { get; set; } = string.Empty;
	}
}