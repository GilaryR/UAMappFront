using SistemaHorario.UI.Dialogs.Materias;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Materias;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Materias
{
    public partial class MateriasView : UserControl
    {
        private readonly MateriasViewModel _viewModel = new();

        public MateriasView()
        {
            InitializeComponent();

            DataContext = _viewModel;

            Loaded += MateriasView_Loaded;
        }

        private async void MateriasView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                await _viewModel.CargarMateriasAsync();

                if (!string.IsNullOrWhiteSpace(_viewModel.MensajeEstado))
                {
                    MessageBox.Show(
                        _viewModel.MensajeEstado,
                        "Materias",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }

                ActualizarResumen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar el módulo de materias:\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private async void BtnAgregarMateria_Click(
            object sender,
            RoutedEventArgs e)
        {
            MateriaDialog dialog = new(
                ModoMateriaDialog.Crear,
                null,
                _viewModel.ObtenerMateriasDisponiblesComoPrerrequisito())
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true ||
                dialog.MateriaResultado == null)
            {
                return;
            }

            bool creada =
                await _viewModel.AgregarMateriaAsync(
                    dialog.MateriaResultado);

            if (!creada)
            {
                MostrarError(_viewModel.MensajeEstado);
                return;
            }

            ActualizarResumen();

            MensajeExitoDialog exito =
                new("Materia creada correctamente.")
                {
                    Owner = Window.GetWindow(this)
                };

            exito.ShowDialog();
        }

        private async void BtnEditar_Click(
            object sender,
            RoutedEventArgs e)
        {
            MateriaItem? materia =
                ObtenerMateriaDesdeBoton(sender);

            if (materia == null)
            {
                return;
            }

            MateriaDialog dialog = new(
                ModoMateriaDialog.Editar,
                materia,
                _viewModel.ObtenerMateriasDisponiblesComoPrerrequisito(materia))
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true ||
                dialog.MateriaResultado == null)
            {
                return;
            }

            bool actualizada =
                await _viewModel.ActualizarMateriaAsync(
                    dialog.MateriaResultado);

            if (!actualizada)
            {
                MostrarError(_viewModel.MensajeEstado);
                return;
            }

            ActualizarResumen();

            MensajeExitoDialog exito =
                new("Materia actualizada correctamente.")
                {
                    Owner = Window.GetWindow(this)
                };

            exito.ShowDialog();
        }

        private void BtnVer_Click(
            object sender,
            RoutedEventArgs e)
        {
            MateriaItem? materia =
                ObtenerMateriaDesdeBoton(sender);

            if (materia == null)
            {
                return;
            }

            MateriaDialog dialog = new(
                ModoMateriaDialog.Ver,
                materia,
                _viewModel.ObtenerMateriasDisponiblesComoPrerrequisito(materia))
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();
        }

        private async void BtnInactivar_Click(object sender, RoutedEventArgs e)
        {
            MateriaItem? materia =
                ObtenerMateriaDesdeBoton(sender);

            if (materia == null)
            {
                return;
            }

            EliminarConfirmacionDialog dialogDesactivar =
                new(
                    "DESACTIVAR",
                    "¿Estás seguro que deseas desactivar?\nDebes escribir la palabra desactivar.",
                    "desactivar")
                {
                    Owner = Window.GetWindow(this)
                };

            if (dialogDesactivar.ShowDialog() != true)
            {
                return;
            }

            bool inactivada =
                await _viewModel.InactivarMateriaAsync(materia);

            if (!inactivada)
            {
                MostrarError(_viewModel.MensajeEstado);
                return;
            }

            ActualizarResumen();

            MensajeExitoDialog exito =
                new("Materia inactivada correctamente.")
                {
                    Owner = Window.GetWindow(this)
                };

            exito.ShowDialog();
        }

        private async void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            MateriaItem? materia =
                ObtenerMateriaDesdeBoton(sender);

            if (materia == null)
            {
                return;
            }

            ConfirmacionDialog confirmacion =
                new(
                    "Reactivar materia",
                    "¿Está seguro que desea reactivar esta materia?")
                {
                    Owner = Window.GetWindow(this)
                };

            if (confirmacion.ShowDialog() != true)
            {
                return;
            }

            bool reactivada =
                await _viewModel.ActivarMateriaAsync(materia);

            if (!reactivada)
            {
                MostrarError(_viewModel.MensajeEstado);
                return;
            }

            ActualizarResumen();

            MensajeExitoDialog exito =
                new("Materia reactivada correctamente.")
                {
                    Owner = Window.GetWindow(this)
                };

            exito.ShowDialog();
        }

        private static MateriaItem? ObtenerMateriaDesdeBoton(
            object sender)
        {
            if (sender is not Button boton)
            {
                return null;
            }

            return boton.DataContext as MateriaItem;
        }

        private void SearchBoxMaterias_BusquedaCambiada(
            object sender,
            RoutedEventArgs e)
        {
            if (_viewModel == null)
            {
                return;
            }

            _viewModel.Busqueda =
                SearchBoxMaterias.TextoBusqueda;

            ActualizarResumen();
        }

        private void CmbSemestre_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (_viewModel == null)
            {
                return;
            }

            if (CmbSemestre.SelectedItem is not ComboBoxItem item)
            {
                return;
            }

            _viewModel.SemestreSeleccionado =
                item.Content?.ToString() ?? "Todos";

            ActualizarResumen();
        }

        private void CmbEstado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (_viewModel == null)
            {
                return;
            }

            if (CmbEstado.SelectedItem is not ComboBoxItem item)
            {
                return;
            }

            _viewModel.EstadoSeleccionado =
                item.Content?.ToString() ?? "Todos";

            ActualizarResumen();
        }

        private void BtnLimpiarFiltros_Click(
            object sender,
            RoutedEventArgs e)
        {
            SearchBoxMaterias.Limpiar();

            CmbSemestre.SelectedIndex = 0;
            CmbEstado.SelectedIndex = 0;

            _viewModel.LimpiarFiltros();

            ActualizarResumen();
        }

        private void ActualizarResumen()
        {
            if (TxtResumen == null || _viewModel == null)
            {
                return;
            }

            TxtResumen.Text =
                $"Mostrando {_viewModel.MateriasFiltradas.Count} de {_viewModel.Materias.Count} materias";
        }

        private static void MostrarError(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
} 