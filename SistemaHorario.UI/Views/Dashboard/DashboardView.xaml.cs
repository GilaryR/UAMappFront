using SistemaHorario.UI.ViewModels.Dashboard;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Dashboard
{
	/// <summary>
	/// Vista principal del Dashboard.
	///
	/// Esta vista únicamente renderiza información preparada
	/// por DashboardViewModel.
	///
	/// El Dashboard queda preparado para futuras conexiones:
	///
	/// - GET /api/dashboard/resumen
	/// - Endpoint futuro de últimos horarios generados
	///
	/// Actualmente utiliza datos temporales.
	/// </summary>
	public partial class DashboardView : UserControl
	{
		/// <summary>
		/// ViewModel asociado al Dashboard.
		/// </summary>
		private readonly DashboardViewModel _viewModel;

		/// <summary>
		/// Constructor principal.
		/// </summary>
		public DashboardView()
		{
			InitializeComponent();

			_viewModel = new DashboardViewModel();

			DataContext = _viewModel;

			ConfigurarCards();
		}

		/// <summary>
		/// Configura visualmente las cards resumen.
		///
		/// Los valores actuales son temporales.
		///
		/// TODO:
		/// Reemplazar cuando se confirme la estructura real
		/// de GET /api/dashboard/resumen.
		/// </summary>
		private void ConfigurarCards()
		{
			CardMaterias.Configurar(
				"Materias",
				_viewModel.TotalMaterias,
				"/Assets/Icons/IcMateriasCircI.png"
			);

			CardPlanes.Configurar(
				"Planes",
				_viewModel.TotalPlanesAcademicos,
				"/Assets/Icons/IcPlanCircI.png"
			);

			CardUsuarios.Configurar(
				"Usuarios",
				_viewModel.TotalUsuarios,
				"/Assets/Icons/IcDocentesCircI.png"
			);

			CardGrupos.Configurar(
				"Grupos",
				_viewModel.TotalGrupos,
				"/Assets/Icons/IcGruposCircI.png"
			);
		}
	}
}