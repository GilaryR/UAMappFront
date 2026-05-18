using SistemaHorario.UI.Dialogs.GruposAcademicos;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.GruposAcademicos;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.GruposAcademicos
{
    public partial class GruposAcademicosView : UserControl
    {
        private readonly GruposAcademicosViewModel _viewModel = new();

        public GruposAcademicosView()
        {
            InitializeComponent();

            DataContext = _viewModel;

            CmbEstado.SelectedValue = "Todos";
            CmbJornada.SelectedValue = "Todas";

            ActualizarBotonesPaginacion();
        }

        private void Filtrar_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.EstadoFiltro = CmbEstado.SelectedValue?.ToString() ?? "Todos";
            _viewModel.JornadaFiltro = CmbJornada.SelectedValue?.ToString() ?? "Todas";

            _viewModel.AplicarFiltros();

            ActualizarBotonesPaginacion();
        }

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LimpiarFiltros();

            CmbEstado.SelectedValue = "Todos";
            CmbJornada.SelectedValue = "Todas";

            ActualizarBotonesPaginacion();
        }

        private void NuevoGrupo_Click(object sender, RoutedEventArgs e)
        {
            GrupoAcademicoDialog dialog = new(
                ModoGrupoAcademicoDialog.Crear,
                null,
                _viewModel.MateriasDisponibles
            )
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
                return;

            _viewModel.AgregarGrupo(dialog.GrupoResultado);

            ActualizarBotonesPaginacion();

            MostrarExito("Grupo académico creado");
        }

        private void VerGrupo_Click(object sender, RoutedEventArgs e)
        {
            GrupoAcademicoItem? grupo = ObtenerGrupoDesdeBoton(sender);

            if (grupo == null)
                return;

            GrupoAcademicoDialog dialog = new(
                ModoGrupoAcademicoDialog.Ver,
                grupo,
                _viewModel.MateriasDisponibles
            )
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();
        }

        private void EditarGrupo_Click(object sender, RoutedEventArgs e)
        {
            GrupoAcademicoItem? grupo = ObtenerGrupoDesdeBoton(sender);

            if (grupo == null)
                return;

            GrupoAcademicoDialog dialog = new(
                ModoGrupoAcademicoDialog.Editar,
                grupo,
                _viewModel.MateriasDisponibles
            )
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
                return;

            _viewModel.ActualizarGrupo(dialog.GrupoResultado);

            ActualizarBotonesPaginacion();

            MostrarExito("Grupo académico actualizado");
        }

        private void EliminarGrupo_Click(object sender, RoutedEventArgs e)
        {
            GrupoAcademicoItem? grupo = ObtenerGrupoDesdeBoton(sender);

            if (grupo == null)
                return;

            EliminarConfirmacionDialog dialog = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
                return;

            _viewModel.EliminarGrupo(grupo.IdGrupoAcademico);

            ActualizarBotonesPaginacion();

            MostrarExito("Grupo académico eliminado");
        }

        private void Anterior_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PaginaAnterior();

            ActualizarBotonesPaginacion();
        }

        private void Siguiente_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SiguientePagina();

            ActualizarBotonesPaginacion();
        }

        private static GrupoAcademicoItem? ObtenerGrupoDesdeBoton(object sender)
        {
            if (sender is not Button button)
                return null;

            return button.CommandParameter as GrupoAcademicoItem;
        }

        private void ActualizarBotonesPaginacion()
        {
            BtnAnterior.IsEnabled = _viewModel.PaginaActual > 1;
            BtnSiguiente.IsEnabled = _viewModel.PaginaActual < _viewModel.TotalPaginas;

            BtnAnterior.Opacity = BtnAnterior.IsEnabled ? 1 : 0.45;
            BtnSiguiente.Opacity = BtnSiguiente.IsEnabled ? 1 : 0.45;
        }

        private void MostrarExito(string mensaje)
        {
            MensajeExitoDialog dialog = new(mensaje)
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();
        }
    }
}