using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using SistemaHorario.UI.ViewModels.Horarios;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SistemaHorario.UI.Views.Horarios
{
    /// <summary>
    /// Muestra el horario semanal de un grupo o de un docente.
    /// </summary>
    public partial class VistaPreviaHorarioView : UserControl
    {
        private readonly HorariosApiService _horariosApi = new();
        private readonly VistaPreviaHorarioViewModel _viewModel;
        private readonly bool _modoAprobacion;

        private BloqueHorarioItem? _bloqueArrastrado;

        private readonly string[] _dias =
        {
            "Lunes",
            "Martes",
            "Miércoles",
            "Jueves",
            "Viernes",
            "Sábado"
        };

        private readonly string[] _horas =
        {
            "07:00 - 09:00",
            "07:30 - 09:30",
            "08:00 - 10:00",
            "08:30 - 10:30",
            "09:00 - 11:00",
            "09:30 - 11:30",
            "10:00 - 12:00",
            "12:00 - 14:00",
            "14:00 - 16:00",
            "14:30 - 16:30",
            "15:00 - 17:00",
            "15:30 - 17:30",
            "16:00 - 18:00",
            "18:00 - 18:30",
            "18:30 - 20:30",
            "19:00 - 21:00",
            "19:30 - 21:30",
            "20:00 - 22:00",
            "20:30 - 22:30"
        };

        public VistaPreviaHorarioView(
            HorarioItem horario,
            bool modoEdicion = false,
            bool modoAprobacion = false)
        {
            InitializeComponent();

            _modoAprobacion = modoAprobacion;
            _viewModel = new VistaPreviaHorarioViewModel(horario, modoEdicion);

            ConfigurarEncabezado();
            ConstruirHorario();
            Loaded += VistaPreviaHorarioView_Loaded;
        }

        public VistaPreviaHorarioView(
            HorarioItem horario,
            bool modoEdicion,
            bool modoAprobacion,
            string tipoVista)
            : this(PrepararHorarioSegunVista(horario, tipoVista), modoEdicion, modoAprobacion)
        {
        }

        private static HorarioItem PrepararHorarioSegunVista(
            HorarioItem horario,
            string tipoVista)
        {
            if (!string.IsNullOrWhiteSpace(tipoVista) &&
                tipoVista.Trim().ToLower() == "docente")
            {
                horario.EsHorarioDocente = true;
            }

            return horario;
        }

        public VistaPreviaHorarioView(DocenteItem docente)
            : this(new HorarioItem
            {
                IdDocente = docente.IdDocente,
                Nombre = $"Horario docente - {docente.NombreCompleto}",
                Grupo = docente.Identificacion,
                Tipo = "Docente",
                Jornada = string.Empty,
                Estado = docente.Estado,
                EsHorarioDocente = true
            }, false, false)
        {
        }

        private async void VistaPreviaHorarioView_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.CargarBloquesAsync();

            if (_viewModel.ModoEdicion && !_viewModel.Horario.EsHorarioDocente)
            {
                await _viewModel.CargarFranjasAsync();
            }

            if (!string.IsNullOrWhiteSpace(_viewModel.MensajeEstado))
            {
                MessageBox.Show(
                    _viewModel.MensajeEstado,
                    "Horario",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            ConstruirHorario();
        }

        private void ConfigurarEncabezado()
        {
            if (_viewModel.Horario.EsHorarioDocente)
            {
                TxtTitulo.Text = "Horario del docente";
                TxtNombreHorario.Text = _viewModel.Horario.Nombre;
                TxtDetalleHorario.Text = "Clases asignadas al docente según horarios generados por grupo";
            }
            else
            {
                TxtTitulo.Text = _viewModel.ModoEdicion
                    ? "Editar horario del grupo"
                    : "Horario del grupo";

                TxtNombreHorario.Text = _viewModel.Horario.Nombre;
                TxtDetalleHorario.Text =
                    $"{_viewModel.Horario.Grupo} · {_viewModel.Horario.Tipo} · {_viewModel.Horario.Jornada} · {_viewModel.Horario.Estado}";
            }

            bool mostrarAprobacion =
                !_viewModel.Horario.EsHorarioDocente &&
                (_modoAprobacion || _viewModel.ModoEdicion);

            BtnAprobar.Visibility = mostrarAprobacion
                ? Visibility.Visible
                : Visibility.Collapsed;

            BtnRechazar.Visibility = mostrarAprobacion
                ? Visibility.Visible
                : Visibility.Collapsed;

            BtnEditar.Visibility =
                !_viewModel.Horario.EsHorarioDocente &&
                _modoAprobacion &&
                !_viewModel.ModoEdicion
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            BtnSalir.Visibility = Visibility.Visible;
        }

        private void ConstruirHorario()
        {
            GridHorario.Children.Clear();
            GridHorario.RowDefinitions.Clear();
            GridHorario.ColumnDefinitions.Clear();

            GridHorario.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });

            foreach (string dia in _dias)
            {
                GridHorario.ColumnDefinitions.Add(
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i <= _horas.Length; i++)
            {
                GridHorario.RowDefinitions.Add(new RowDefinition { Height = new GridLength(70) });
            }

            AgregarCelda("Hora", 0, 0, true);

            for (int i = 0; i < _dias.Length; i++)
            {
                AgregarCelda(_dias[i], 0, i + 1, true);
            }

            for (int fila = 0; fila < _horas.Length; fila++)
            {
                string hora = _horas[fila];

                AgregarCelda(hora, fila + 1, 0, true);

                for (int col = 0; col < _dias.Length; col++)
                {
                    AgregarCeldaContenido(_dias[col], hora, fila + 1, col + 1);
                }
            }
        }

        private void AgregarCelda(string texto, int fila, int columna, bool encabezado)
        {
            Border border = new()
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0.5),
                Background = encabezado
                    ? new SolidColorBrush(Color.FromRgb(242, 242, 242))
                    : Brushes.White,
                Child = new TextBlock
                {
                    Text = texto,
                    FontWeight = encabezado ? FontWeights.Bold : FontWeights.Normal,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                }
            };

            Grid.SetRow(border, fila);
            Grid.SetColumn(border, columna);
            GridHorario.Children.Add(border);
        }

        private void AgregarCeldaContenido(string dia, string rangoHora, int fila, int columna)
        {
            if (EsBloqueado(rangoHora))
            {
                AgregarBloqueado(fila, columna);
                return;
            }

            string horaInicio = ObtenerHoraInicio(rangoHora);

            BloqueHorarioItem? bloque = _viewModel.Bloques.FirstOrDefault(b =>
                NormalizarDia(b.Dia) == NormalizarDia(dia) &&
                b.HoraInicio == horaInicio);

            Border border = new()
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0.5),
                Background = Brushes.White,
                AllowDrop = _viewModel.ModoEdicion && !_viewModel.Horario.EsHorarioDocente,
                Tag = new DatosCeldaHorario(dia, rangoHora)
            };

            if (_viewModel.ModoEdicion && !_viewModel.Horario.EsHorarioDocente)
            {
                border.Drop += Celda_Drop;
            }

            if (bloque != null)
            {
                border.Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(bloque.ColorVisual));

                border.Child = new TextBlock
                {
                    Text = bloque.TextoCelda,
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };

                if (_viewModel.ModoEdicion && !_viewModel.Horario.EsHorarioDocente)
                {
                    border.Cursor = Cursors.Hand;
                    border.Tag = bloque;
                    border.MouseMove += Bloque_MouseMove;
                    border.AllowDrop = true;
                    border.Drop += Celda_Drop;
                }
            }

            Grid.SetRow(border, fila);
            Grid.SetColumn(border, columna);
            GridHorario.Children.Add(border);
        }

        private static bool EsBloqueado(string rangoHora)
        {
            return rangoHora == "12:00 - 14:00" ||
                   rangoHora == "18:00 - 18:30";
        }

        private void AgregarBloqueado(int fila, int columna)
        {
            Border border = new()
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0.5),
                Background = new SolidColorBrush(Color.FromRgb(217, 217, 217)),
                Child = new TextBlock
                {
                    Text = "Sin clase",
                    Foreground = Brushes.DimGray,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };

            Grid.SetRow(border, fila);
            Grid.SetColumn(border, columna);
            GridHorario.Children.Add(border);
        }

        private void Bloque_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_viewModel.ModoEdicion || _viewModel.Horario.EsHorarioDocente)
            {
                return;
            }

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (sender is not Border border || border.Tag is not BloqueHorarioItem bloque)
            {
                return;
            }

            _bloqueArrastrado = bloque;
            DragDrop.DoDragDrop(border, bloque, DragDropEffects.Move);
        }

        private async void Celda_Drop(object sender, DragEventArgs e)
        {
            if (!_viewModel.ModoEdicion || _viewModel.Horario.EsHorarioDocente)
            {
                return;
            }

            if (_bloqueArrastrado == null || sender is not Border border)
            {
                return;
            }

            DatosCeldaHorario? destino = ObtenerDatosDestino(border);
            if (destino == null)
            {
                return;
            }

            if (EsBloqueado(destino.RangoHora))
            {
                MessageBox.Show(
                    "No se puede mover una clase a una franja bloqueada.",
                    "Franja no permitida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            string diaOriginal = _bloqueArrastrado.Dia;
            string horaOriginal = _bloqueArrastrado.HoraInicio;
            int idFranjaOriginal = _bloqueArrastrado.IdFranjaHoraria;

            _bloqueArrastrado.Dia = destino.Dia;
            _bloqueArrastrado.HoraInicio = ObtenerHoraInicio(destino.RangoHora);

            BloqueHorarioItem bloqueGuardar = _bloqueArrastrado;
            _bloqueArrastrado = null;

            bool ok = await _viewModel.GuardarBloqueAsync(bloqueGuardar);
            if (!ok)
            {
                bloqueGuardar.Dia = diaOriginal;
                bloqueGuardar.HoraInicio = horaOriginal;
                bloqueGuardar.IdFranjaHoraria = idFranjaOriginal;

                MessageBox.Show(
                    "No se pudo mover el bloque:\n" + _viewModel.MensajeEstado,
                    "Movimiento no permitido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            await _viewModel.CargarBloquesAsync();
            ConstruirHorario();
        }

        private static DatosCeldaHorario? ObtenerDatosDestino(Border border)
        {
            if (border.Tag is DatosCeldaHorario datosCelda)
            {
                return datosCelda;
            }

            if (border.Tag is BloqueHorarioItem bloque)
            {
                return new DatosCeldaHorario(
                    bloque.Dia,
                    $"{bloque.HoraInicio} - {bloque.HoraFinal}");
            }

            return null;
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            NavegarA(new VistaPreviaHorarioView(_viewModel.Horario, true, true));
        }

        private async void BtnAprobar_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Horario.IdHorario <= 0)
            {
                MessageBox.Show(
                    "No se puede aprobar el horario porque no tiene un identificador válido.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var resp = await _horariosApi.AprobarHorarioAsync(_viewModel.Horario.IdHorario);

            if (!resp.Success)
            {
                MessageBox.Show(
                    "No se pudo aprobar el horario:\n" + resp.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MensajeExitoDialog dialog = new("Horario aprobado correctamente.")
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();
            NavegarA(new HorariosView());
        }

        private async void BtnRechazar_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Horario.IdHorario <= 0)
            {
                MessageBox.Show(
                    "No se puede rechazar el horario porque no tiene un identificador válido.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            string motivo = SolicitarMotivoRechazo();
            if (string.IsNullOrWhiteSpace(motivo))
            {
                return;
            }

            var resp = await _horariosApi.RechazarHorarioAsync(
                _viewModel.Horario.IdHorario,
                motivo);

            if (!resp.Success)
            {
                MessageBox.Show(
                    "No se pudo rechazar el horario:\n" + resp.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MessageBox.Show(
                "Horario rechazado correctamente.",
                "Rechazo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            NavegarA(new HorariosView());
        }

        private string SolicitarMotivoRechazo()
        {
            Window ventana = new()
            {
                Title = "Motivo de rechazo",
                Width = 420,
                Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize
            };

            StackPanel panel = new() { Margin = new Thickness(18) };

            TextBlock texto = new()
            {
                Text = "Escribe el motivo de rechazo del horario:",
                Margin = new Thickness(0, 0, 0, 8),
                FontWeight = FontWeights.SemiBold
            };

            TextBox caja = new()
            {
                Height = 70,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true
            };

            StackPanel botones = new()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 16, 0, 0)
            };

            Button cancelar = new()
            {
                Content = "Cancelar",
                Width = 90,
                Height = 34,
                Margin = new Thickness(0, 0, 10, 0)
            };

            Button aceptar = new()
            {
                Content = "Rechazar",
                Width = 100,
                Height = 34,
                Background = new SolidColorBrush(Color.FromRgb(176, 0, 32)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0)
            };

            string motivo = string.Empty;

            cancelar.Click += (_, _) =>
            {
                ventana.DialogResult = false;
                ventana.Close();
            };

            aceptar.Click += (_, _) =>
            {
                motivo = caja.Text.Trim();

                if (string.IsNullOrWhiteSpace(motivo))
                {
                    MessageBox.Show(
                        "Debe escribir un motivo de rechazo.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                ventana.DialogResult = true;
                ventana.Close();
            };

            botones.Children.Add(cancelar);
            botones.Children.Add(aceptar);
            panel.Children.Add(texto);
            panel.Children.Add(caja);
            panel.Children.Add(botones);
            ventana.Content = panel;

            bool? resultado = ventana.ShowDialog();
            return resultado == true ? motivo : string.Empty;
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Horario.EsHorarioDocente)
            {
                NavegarA(new SistemaHorario.UI.Views.Docentes.DocentesView());
                return;
            }

            NavegarA(new HorariosView());
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
                if (actual is ContentControl content && content.Name == "ContentArea")
                {
                    return content;
                }

                actual = System.Windows.Media.VisualTreeHelper.GetParent(actual);
            }

            return null;
        }

        private static string ObtenerHoraInicio(string rangoHora)
        {
            return rangoHora.Split('-')[0].Trim();
        }

        private static string NormalizarDia(string dia)
        {
            return dia.Trim()
                .ToLower()
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u");
        }

        private class DatosCeldaHorario
        {
            public string Dia { get; }

            public string RangoHora { get; }

            public DatosCeldaHorario(string dia, string rangoHora)
            {
                Dia = dia;
                RangoHora = rangoHora;
            }
        }
    }
}
