using SistemaHorario.UI.Models.UI;
using System.Collections.Generic;

namespace SistemaHorario.UI.ViewModels.Docentes
{
	public class AgregarEditarDocenteViewModel
	{
		public DocenteItem Docente { get; set; }

		public List<DisponibilidadDocenteItem> Disponibilidad { get; set; }

		public bool EsEdicion { get; set; }

		public AgregarEditarDocenteViewModel()
		{
			Docente = new DocenteItem { Estado = "Activo" };
			Disponibilidad = DocentesMockStore.CrearDisponibilidadBase();
			EsEdicion = false;
		}

		public AgregarEditarDocenteViewModel(DocenteItem docente)
		{
			Docente = new DocenteItem
			{
				IdDocente = docente.IdDocente,
				NombreCompleto = docente.NombreCompleto,
				Identificacion = docente.Identificacion,
				CorreoInstitucional = docente.CorreoInstitucional,
				Materias = docente.Materias,
				Estado = docente.Estado
			};
			Disponibilidad = new List<DisponibilidadDocenteItem>();
			EsEdicion = true;
		}
	}
}
