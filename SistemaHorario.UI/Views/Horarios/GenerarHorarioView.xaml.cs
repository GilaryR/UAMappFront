using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Horarios;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Horarios
{
    public partial class GenerarHorarioView : UserControl
    {
        private readonly GenerarHorarioViewModel _viewModel;

        public GenerarHorarioView()
        {
            InitializeComponent();

            _viewModel = new GenerarHorarioViewModel();

            CmbGrupo.ItemsSource = _viewModel.Grupos;

            Loaded += GenerarHorarioView_Loaded;
        }

        private async void GenerarHorarioView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await _viewModel.CargarGruposAsync();

            if (_viewModel.Grupos.Count > 0)
            {
                CmbGrupo.SelectedIndex = 0;
                return;
            }

            if (!string.IsNullOrWhiteSpace(_viewModel.MensajeEstado))
            {
                MessageBox.Show(
                    _viewModel.MensajeEstado,
                    "Grupos no disponibles",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        private async void BtnGenerar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!FormularioEsValido())
            {
                return;
            }

            GrupoHorarioOption grupoSeleccionado =
                (GrupoHorarioOption)CmbGrupo.SelectedItem;

            CambiarEstadoBotonGenerar(false);

            bool generado =
                await _viewModel.GenerarHorarioAsync(
                    grupoSeleccionado.IdGrupo
                );

            if (!generado)
            {
                CambiarEstadoBotonGenerar(true);

                MessageBox.Show(
                    "Error al generar el horario:\n" +
                    _viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                return;
            }

            HorarioItem? horarioGenerado =
                await _viewModel.ObtenerUltimoHorarioGeneradoAsync(
                    grupoSeleccionado.IdGrupo
                );

            CambiarEstadoBotonGenerar(true);

            if (horarioGenerado == null)
            {
                MessageBox.Show(
                    _viewModel.MensajeEstado,
                    "Horario generado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                NavegarA(new HorariosView());
                return;
            }

            NavegarA(
                new VistaPreviaHorarioView(
                    horarioGenerado,
                    false,
                    true
                )
            );
        }

        private bool FormularioEsValido()
        {
            if (CmbGrupo.SelectedItem is not GrupoHorarioOption)
            {
                MessageBox.Show(
                    "Debe seleccionar un grupo antes de generar el horario.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            return true;
        }

        private void CambiarEstadoBotonGenerar(bool habilitado)
        {
            BtnGenerar.IsEnabled = habilitado;
            BtnGenerar.Content = habilitado
                ? "Generar horario"
                : "Generando...";
        }

        private void NavegarA(UserControl vista)
        {
            ContentControl? contentArea = BuscarContentArea();

            if (contentArea == null)
            {
                return;
            }

            contentArea.Content = vista;
        }

        private ContentControl? BuscarContentArea()
        {
            DependencyObject? actual = this;

            while (actual != null)
            {
                if (actual is ContentControl content &&
                    content.Name == "ContentArea")
                {
                    return content;
                }

                actual =
                    System.Windows.Media.VisualTreeHelper.GetParent(actual);
            }

            return null;
        }
    }
}