using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using SistemaHorario.UI.ViewModels.Docentes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SistemaHorario.UI.Dialogs.Docentes
{
    public partial class AgregarEditarDocenteDialog : Window
    {
        private readonly AgregarEditarDocenteViewModel _viewModel;
        private readonly bool _soloLectura;

        // Selected materias tracked as {IdMateria, Nombre} pairs
        private readonly ObservableCollection<MateriaItem> _materiasSeleccionadas = new();
        private List<MateriaItem> _todasLasMaterias = new();

        public AgregarEditarDocenteDialog()
        {
            InitializeComponent();
            _viewModel = new AgregarEditarDocenteViewModel();
            _soloLectura = false;
            ConfigurarModo();
            CargarCamposBasicos();
            Loaded += Dialog_Loaded;
        }

        public AgregarEditarDocenteDialog(DocenteItem docente, bool soloLectura = false)
        {
            InitializeComponent();
            _viewModel = new AgregarEditarDocenteViewModel(docente);
            _soloLectura = soloLectura;
            ConfigurarModo();
            CargarCamposBasicos();
            Loaded += Dialog_Loaded;
        }

        private async void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var api = new MateriasApiService();
                var resp = await api.ObtenerMateriasAsync();

                if (resp.Success && resp.Data != null)
                    _todasLasMaterias = resp.Data.Where(m => m.Activa).ToList();
            }
            catch { }

            // Fallback: if API returned nothing, use mock store so the combo is never blank
            if (_todasLasMaterias.Count == 0)
            {
                int fakeId = -1;
                _todasLasMaterias = DocentesMockStore.ObtenerMateriasDisponibles()
                    .Select(nombre => new MateriaItem
                    {
                        IdMateria = fakeId--,
                        Nombre = nombre,
                        Activa = true
                    }).ToList();
            }

            // Pre-select by ID (when IdsMateria populated) or by name (when loaded from list)
            if (_viewModel.Docente.IdsMateria.Count > 0)
            {
                foreach (var id in _viewModel.Docente.IdsMateria)
                {
                    var m = _todasLasMaterias.FirstOrDefault(x => x.IdMateria == id);
                    if (m != null && !_materiasSeleccionadas.Any(s => s.IdMateria == id))
                        _materiasSeleccionadas.Add(m);
                }
            }
            else if (!string.IsNullOrWhiteSpace(_viewModel.Docente.Materias))
            {
                var nombres = _viewModel.Docente.Materias
                    .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => n.Trim())
                    .ToHashSet();

                foreach (var m in _todasLasMaterias)
                {
                    if (nombres.Contains(m.Nombre))
                        _materiasSeleccionadas.Add(m);
                }
            }

            ActualizarComboMaterias();
            DibujarMateriasSeleccionadas();
        }

        private void ConfigurarModo()
        {
            if (_soloLectura)
            {
                TxtTitulo.Text = "Detalle docente";
                TxtSubtitulo.Text = "Información general del docente";
                BtnGuardar.Visibility = Visibility.Collapsed;
                BloquearFormulario();
                return;
            }

            if (_viewModel.EsEdicion)
            {
                TxtTitulo.Text = "Editar docente";
                BtnGuardar.Content = "Guardar cambios";
            }
        }

        private void BloquearFormulario()
        {
            TxtNombreCompleto.IsReadOnly = true;
            TxtIdentificacion.IsReadOnly = true;
            TxtCorreo.IsReadOnly = true;
            CmbEstado.IsEnabled = false;
            CmbMateriaDisponible.IsEnabled = false;
            BtnAgregarMateria.Visibility = Visibility.Collapsed;
            BtnDisponibilidad.IsEnabled = false;
        }

        private void CargarCamposBasicos()
        {
            TxtNombreCompleto.Text = _viewModel.Docente.NombreCompleto;
            TxtIdentificacion.Text = _viewModel.Docente.Identificacion;
            TxtCorreo.Text = _viewModel.Docente.CorreoInstitucional;

            foreach (ComboBoxItem item in CmbEstado.Items)
            {
                if (item.Content?.ToString() == _viewModel.Docente.Estado)
                {
                    CmbEstado.SelectedItem = item;
                    break;
                }
            }
        }

        private void ActualizarComboMaterias()
        {
            var seleccionadosIds = _materiasSeleccionadas.Select(m => m.IdMateria).ToHashSet();
            var disponibles = _todasLasMaterias
                .Where(m => !seleccionadosIds.Contains(m.IdMateria))
                .ToList();

            CmbMateriaDisponible.DisplayMemberPath = "Nombre";
            CmbMateriaDisponible.ItemsSource = disponibles;

            if (disponibles.Count > 0)
                CmbMateriaDisponible.SelectedIndex = 0;
        }

        private void DibujarMateriasSeleccionadas()
        {
            PanelMateriasSeleccionadas.Children.Clear();

            foreach (var materia in _materiasSeleccionadas)
            {
                Border chip = new()
                {
                    Background = new SolidColorBrush(Color.FromRgb(219, 234, 254)),
                    CornerRadius = new CornerRadius(14),
                    Padding = new Thickness(10, 5, 8, 5),
                    Margin = new Thickness(0, 0, 8, 8)
                };

                StackPanel contenido = new() { Orientation = Orientation.Horizontal };

                TextBlock texto = new()
                {
                    Text = materia.Nombre,
                    Foreground = new SolidColorBrush(Color.FromRgb(30, 64, 175)),
                    FontWeight = FontWeights.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center
                };

                contenido.Children.Add(texto);

                if (!_soloLectura)
                {
                    Button quitar = new()
                    {
                        Content = "x",
                        Width = 20,
                        Height = 20,
                        Margin = new Thickness(8, 0, 0, 0),
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        Foreground = new SolidColorBrush(Color.FromRgb(30, 64, 175)),
                        FontWeight = FontWeights.Bold,
                        Cursor = Cursors.Hand,
                        Tag = materia
                    };
                    quitar.Click += BtnQuitarMateria_Click;
                    contenido.Children.Add(quitar);
                }

                chip.Child = contenido;
                PanelMateriasSeleccionadas.Children.Add(chip);
            }
        }

        private void BtnAgregarMateria_Click(object sender, RoutedEventArgs e)
        {
            if (CmbMateriaDisponible.SelectedItem is not MateriaItem materia)
                return;

            if (_materiasSeleccionadas.Any(m => m.IdMateria == materia.IdMateria))
                return;

            _materiasSeleccionadas.Add(materia);
            DibujarMateriasSeleccionadas();
            ActualizarComboMaterias();
        }

        private void BtnQuitarMateria_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not MateriaItem materia)
                return;

            var existing = _materiasSeleccionadas.FirstOrDefault(m => m.IdMateria == materia.IdMateria);
            if (existing != null)
                _materiasSeleccionadas.Remove(existing);

            DibujarMateriasSeleccionadas();
            ActualizarComboMaterias();
        }

        private void BtnDisponibilidad_Click(object sender, RoutedEventArgs e)
        {
            DisponibilidadDocenteDialog dialog = new(_viewModel.Disponibilidad) { Owner = this };
            if (dialog.ShowDialog() != true) return;
            _viewModel.Disponibilidad = dialog.DisponibilidadResultado;
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!FormularioEsValido())
                return;

            _viewModel.Docente.NombreCompleto = TxtNombreCompleto.Text.Trim();
            _viewModel.Docente.Identificacion = TxtIdentificacion.Text.Trim();
            _viewModel.Docente.CorreoInstitucional = TxtCorreo.Text.Trim();
            _viewModel.Docente.Materias = string.Join(", ", _materiasSeleccionadas.Select(m => m.Nombre));
            _viewModel.Docente.IdsMateria = _materiasSeleccionadas.Select(m => m.IdMateria).ToList();

            if (CmbEstado.SelectedItem is ComboBoxItem item)
                _viewModel.Docente.Estado = item.Content?.ToString() ?? "Activo";

            var api = new DocentesApiService();
            SistemaHorarios.Application.Common.ApiResponse<string> resp;

            if (_viewModel.EsEdicion)
                resp = await api.ActualizarDocenteAsync(_viewModel.Docente);
            else
                resp = await api.CrearDocenteAsync(_viewModel.Docente);

            if (!resp.Success)
            {
                MessageBox.Show("Error al guardar: " + resp.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MensajeExitoDialog exito = new(_viewModel.EsEdicion ? "Docente actualizado" : "Docente creado")
            {
                Owner = this
            };
            exito.ShowDialog();

            DialogResult = true;
            Close();
        }

        private bool FormularioEsValido()
        {
            if (string.IsNullOrWhiteSpace(TxtNombreCompleto.Text))
            {
                MessageBox.Show("El nombre completo es obligatorio.", "⚠ Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtIdentificacion.Text))
            {
                MessageBox.Show("La identificación es obligatoria.", "⚠ Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtCorreo.Text))
            {
                MessageBox.Show("El correo institucional es obligatorio.", "⚠ Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (_materiasSeleccionadas.Count == 0)
            {
                MessageBox.Show("⚠ Debes agregar al menos una materia.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
