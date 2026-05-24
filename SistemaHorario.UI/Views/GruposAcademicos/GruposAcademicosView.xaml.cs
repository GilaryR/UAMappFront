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
            _viewModel.EstadoFiltro =
                CmbEstado.SelectedValue?.ToString() ?? "Todos";

            _viewModel.JornadaFiltro =
                CmbJornada.SelectedValue?.ToString() ?? "Todas";

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

        private async void NuevoGrupo_Click(object sender, RoutedEventArgs e)
        {
            GrupoAcademicoDialog dialog =
                new(ModoGrupoAcademicoDialog.Crear, null)
                {
                    Owner = Window.GetWindow(this)
                };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            bool ok =
                await _viewModel.AgregarGrupoAsync(dialog.GrupoResultado);

            if (!ok)
            {
                MessageBox.Show(
                    "Error: " + _viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                return;
            }

            ActualizarBotonesPaginacion();
            MostrarExito("Grupo académico creado");
        }

        private void VerGrupo_Click(object sender, RoutedEventArgs e)
        {
            GrupoAcademicoItem? grupo =
                ObtenerGrupoDesdeBoton(sender);

            if (grupo == null)
            {
                return;
            }

            GrupoAcademicoDialog dialog =
                new(ModoGrupoAcademicoDialog.Ver, grupo)
                {
                    Owner = Window.GetWindow(this)
                };

            dialog.ShowDialog();
        }

        private async void EditarGrupo_Click(object sender, RoutedEventArgs e)
        {
            GrupoAcademicoItem? grupo =
                ObtenerGrupoDesdeBoton(sender);

            if (grupo == null)
            {
                return;
            }

            GrupoAcademicoDialog dialog =
                new(ModoGrupoAcademicoDialog.Editar, grupo)
                {
                    Owner = Window.GetWindow(this)
                };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            bool ok =
                await _viewModel.ActualizarGrupoAsync(dialog.GrupoResultado);

            if (!ok)
            {
                MessageBox.Show(
                    "Error: " + _viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                return;
            }

            ActualizarBotonesPaginacion();
            MostrarExito("Grupo académico actualizado");
        }

        private async void EliminarGrupo_Click(object sender, RoutedEventArgs e)
        {
            GrupoAcademicoItem? grupo =
                ObtenerGrupoDesdeBoton(sender);

            if (grupo == null)
            {
                return;
            }

            bool estaActivo =
                grupo.Estado.Equals(
                    "Activo",
                    StringComparison.OrdinalIgnoreCase
                );

            string accion =
                estaActivo ? "inactivar" : "reactivar";

            string mensajeExito =
                estaActivo
                    ? "Grupo académico inactivado"
                    : "Grupo académico reactivado";

            MessageBoxResult confirmacion =
                MessageBox.Show(
                    $"¿Deseas {accion} el grupo académico {grupo.Codigo}?",
                    "Confirmar cambio de estado",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

            if (confirmacion != MessageBoxResult.Yes)
            {
                return;
            }

            bool ok =
                await _viewModel.CambiarEstadoGrupoAsync(grupo);

            if (!ok)
            {
                MessageBox.Show(
                    "Error: " + _viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                return;
            }

            ActualizarBotonesPaginacion();
            MostrarExito(mensajeExito);
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
            {
                return null;
            }

            return button.CommandParameter as GrupoAcademicoItem;
        }

        private void ActualizarBotonesPaginacion()
        {
            BtnAnterior.IsEnabled =
                _viewModel.PaginaActual > 1;

            BtnSiguiente.IsEnabled =
                _viewModel.PaginaActual < _viewModel.TotalPaginas;

            BtnAnterior.Opacity =
                BtnAnterior.IsEnabled ? 1 : 0.45;

            BtnSiguiente.Opacity =
                BtnSiguiente.IsEnabled ? 1 : 0.45;
        }

        private void MostrarExito(string mensaje)
        {
            MensajeExitoDialog dialog =
                new(mensaje)
                {
                    Owner = Window.GetWindow(this)
                };

            dialog.ShowDialog();
        }
    }
}