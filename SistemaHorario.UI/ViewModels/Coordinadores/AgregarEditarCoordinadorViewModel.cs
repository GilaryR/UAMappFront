using SistemaHorario.UI.Models.UI;

namespace SistemaHorario.UI.ViewModels.Coordinadores
{
	/// <summary>
	/// ViewModel utilizado para:
	/// - crear coordinadores,
	/// - editar coordinadores.
	/// </summary>
	public class AgregarEditarCoordinadorViewModel
	{
		public CoordinadorItem Coordinador { get; set; }

		public bool EsEdicion { get; set; }

		public AgregarEditarCoordinadorViewModel()
		{
			Coordinador = new CoordinadorItem
			{
				Rol = "Coordinador",
				Estado = "Activo"
			};

			EsEdicion = false;
		}

		public AgregarEditarCoordinadorViewModel(
			CoordinadorItem coordinador)
		{
			Coordinador = coordinador;

			EsEdicion = true;
		}
	}
}