using SistemaHorario.UI.ViewModels.Dashboard;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Dashboard
{
    /// <summary>
    /// Vista principal del Dashboard.
    /// </summary>
    public partial class DashboardView : UserControl
    {
        private readonly DashboardViewModel _viewModel;

        public DashboardView()
        {
            InitializeComponent();

            _viewModel = new DashboardViewModel();
            DataContext = _viewModel;

            Loaded += DashboardView_Loaded;
        }

        private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.CargarResumenAsync();
            ConfigurarCards();
        }

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