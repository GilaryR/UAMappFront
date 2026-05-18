using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Información resumen visual
	/// para cards del módulo coordinadores.
	/// </summary>
	public class ResumenCoordinadorItem
	{
		public int TotalCoordinadores { get; set; }

		public int Activos { get; set; }

		public int Inactivos { get; set; }
	}
}
