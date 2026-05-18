using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Modelo para tarjetas resumen del módulo Docentes.
	/// </summary>
	public class ResumenDocenteItem
	{
		public int TotalDocentes { get; set; }

		public int Activos { get; set; }

		public int Inactivos { get; set; }

		public int Disponibles { get; set; }
	}
}
