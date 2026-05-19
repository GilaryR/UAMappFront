using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Horarios;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SistemaHorario.UI.Views.Horarios
{
    /// <summary>
    /// Vista previa visual del horario generado.
    ///
    /// Permite:
    /// - Ver un horario en solo lectura.
    /// - Aprobar horario.
    /// - Rechazar horario.
    /// - Editar visualmente bloques mediante arrastrar y soltar.
    ///
    /// Actualmente trabaja con datos mock.
    ///
    /// La UI muestra la estructura horaria general de lunes a sábado,
    /// incluyendo franjas bloqueadas institucionales.
    ///
    /// Importante:
    /// La duración real de cada clase, la asignación de materias,
    /// docentes, aulas y horas debe venir desde backend.
    ///
    /// Endpoints futuros:
    /// - GET /api/horarios/{id}/vista-previa
    /// - POST /api/horarios/{id}/aprobar
    /// - POST /api/horarios/{id}/rechazar
    /// - PUT /api/horarios/{id}/asignatura
    /// </summary>
    public partial class VistaPreviaHorarioView : UserControl
    {
        private readonly VistaPreviaHorarioViewModel _viewModel;
        private readonly bool _modoAprobacion;

        private BloqueHorarioItem? _bloqueArrastrado;

        private readonly string[] _dias =
        [
            "Lunes",
            "Martes",
            "Miércoles",
            "Jueves",
            "Viernes",
            "Sábado"
        ];

        private readonly string[] _horas =
        [
            "7:00 - 8:00",
            "8:00 - 9:00",
            "9:00 - 10:00",
            "10:00 - 11:00",
            "11:00 - 12:00",
            "12:00 - 2:00",
            "2:00 - 3:00",
            "3:00 - 4:00",
            "4:00 - 5:00",
            "5:00 - 6:00",
            "6:00 - 6:30",
            "6:30 - 7:30",
            "7:30 - 8:30",
            "8:30 - 9:30",
            "9:30 - 10:30"
        ];

        public VistaPreviaHorarioView(
            HorarioItem horario,
            bool modoEdicion = false,
            bool modoAprobacion = false)
        {
            InitializeComponent();

            _modoAprobacion = modoAprobacion;

            _viewModel = new VistaPreviaHorarioViewModel(
                horario,
                modoEdicion);

            ConfigurarEncabezado();
            ConstruirHorario();
            Loaded += VistaPreviaHorarioView_Loaded;
        }

        private async void VistaPreviaHorarioView_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.CargarBloquesAsync();
            if (_viewModel.Bloques.Count > 0)
                ConstruirHorario();
        }

        /// <summary>
        /// Configura títulos y botones según el modo de uso.
        ///
        /// Ver desde tabla:
        /// - Solo muestra Salir.
        ///
        /// Generar:
        /// - Muestra Aprobar, Editar, Rechazar y Salir.
        ///
        /// Editar:
        /// - Muestra Aprobar, Rechazar y Salir.
        /// - Oculta Editar porque ya está en modo edición.
        /// </summary>
        private void ConfigurarEncabezado()
        {
            TxtTitulo.Text = _viewModel.ModoEdicion
                ? "Editar horario generado"
                : "Vista previa de horario";

            TxtNombreHorario.Text = _viewModel.Horario.Nombre;

            TxtDetalleHorario.Text =
                $"{_viewModel.Horario.Grupo} · {_viewModel.Horario.Tipo} · {_viewModel.Horario.Jornada}";

            bool mostrarAprobacion =
                _modoAprobacion || _viewModel.ModoEdicion;

            BtnAprobar.Visibility = mostrarAprobacion
                ? Visibility.Visible
                : Visibility.Collapsed;

            BtnRechazar.Visibility = mostrarAprobacion
                ? Visibility.Visible
                : Visibility.Collapsed;

            BtnEditar.Visibility =
                _modoAprobacion && !_viewModel.ModoEdicion
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            BtnSalir.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Construye dinámicamente la tabla semanal.
        ///
        /// La tabla siempre muestra todas las franjas institucionales.
        /// Los bloques reales de clase se posicionan según HoraInicio
        /// y HoraFinal de cada BloqueHorarioItem.
        /// </summary>
        private void ConstruirHorario()
        {
            GridHorario.Children.Clear();
            GridHorario.RowDefinitions.Clear();
            GridHorario.ColumnDefinitions.Clear();

            GridHorario.ColumnDefinitions.Add(
                new ColumnDefinition { Width = new GridLength(110) });

            foreach (string dia in _dias)
            {
                GridHorario.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
            }

            for (int i = 0; i <= _horas.Length; i++)
            {
                GridHorario.RowDefinitions.Add(
                    new RowDefinition { Height = new GridLength(58) });
            }

            AgregarCelda("Hora", 0, 0, true);

            for (int i = 0; i < _dias.Length; i++)
                AgregarCelda(_dias[i], 0, i + 1, true);

            for (int fila = 0; fila < _horas.Length; fila++)
            {
                string hora = _horas[fila];

                AgregarCelda(hora, fila + 1, 0, true);

                for (int col = 0; col < _dias.Length; col++)
                {
                    AgregarCeldaContenido(
                        _dias[col],
                        hora,
                        fila + 1,
                        col + 1);
                }
            }
        }

        private void AgregarCelda(
            string texto,
            int fila,
            int columna,
            bool encabezado)
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
                    FontWeight = encabezado
                        ? FontWeights.Bold
                        : FontWeights.Normal,
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

        private void AgregarCeldaContenido(
            string dia,
            string rangoHora,
            int fila,
            int columna)
        {
            if (EsBloqueado(rangoHora))
            {
                AgregarBloqueado(fila, columna);
                return;
            }

            BloqueHorarioItem? bloque =
                _viewModel.Bloques.FirstOrDefault(b =>
                    b.Dia == dia &&
                    rangoHora.StartsWith(b.HoraInicio));

            Border border = new()
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0.5),
                Background = Brushes.White,
                AllowDrop = _viewModel.ModoEdicion,
                Tag = new DatosCeldaHorario(dia, rangoHora)
            };

            if (_viewModel.ModoEdicion)
                border.Drop += Celda_Drop;

            if (bloque != null)
            {
                border.Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(
                        bloque.ColorVisual));

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

                if (_viewModel.ModoEdicion)
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
            return rangoHora == "12:00 - 2:00" ||
                   rangoHora == "6:00 - 6:30";
        }

        private void AgregarBloqueado(
            int fila,
            int columna)
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

        private void Bloque_MouseMove(
            object sender,
            MouseEventArgs e)
        {
            if (!_viewModel.ModoEdicion)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (sender is not Border border)
                return;

            if (border.Tag is not BloqueHorarioItem bloque)
                return;

            _bloqueArrastrado = bloque;

            DragDrop.DoDragDrop(
                border,
                bloque,
                DragDropEffects.Move);
        }

        private void Celda_Drop(
            object sender,
            DragEventArgs e)
        {
            if (!_viewModel.ModoEdicion)
                return;

            if (_bloqueArrastrado == null)
                return;

            if (sender is not Border border)
                return;

            DatosCeldaHorario? destino =
                ObtenerDatosDestino(border);

            if (destino == null)
                return;

            if (EsBloqueado(destino.RangoHora))
            {
                MessageBox.Show(
                    "No se puede mover una clase a una franja bloqueada.",
                    "Franja no permitida",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            _bloqueArrastrado.Dia = destino.Dia;
            _bloqueArrastrado.HoraInicio =
                destino.RangoHora.Split('-')[0].Trim();

            _bloqueArrastrado = null;

            ConstruirHorario();

            // TODO:
            // Al conectar API:
            // PUT /api/horarios/{id}/asignatura
        }

        private static DatosCeldaHorario? ObtenerDatosDestino(
            Border border)
        {
            if (border.Tag is DatosCeldaHorario datosCelda)
                return datosCelda;

            if (border.Tag is BloqueHorarioItem bloque)
            {
                return new DatosCeldaHorario(
                    bloque.Dia,
                    $"{bloque.HoraInicio} - {bloque.HoraFinal}");
            }

            return null;
        }

        private void BtnEditar_Click(
            object sender,
            RoutedEventArgs e)
        {
            NavegarA(new VistaPreviaHorarioView(
                _viewModel.Horario,
                true,
                true));
        }

        private void BtnAprobar_Click(
            object sender,
            RoutedEventArgs e)
        {
            HorariosMockStore.GuardarHorarioAprobado(
                _viewModel.Horario);

            MensajeExitoDialog dialog = new(
                "Horario aprobado.")
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();

            NavegarA(new HorariosView());
        }

        private void BtnRechazar_Click(
            object sender,
            RoutedEventArgs e)
        {
            _viewModel.Horario.Estado = "Rechazado";

            MessageBox.Show(
                "Horario rechazado.",
                "Rechazo",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            NavegarA(new HorariosView());
        }

        private void BtnSalir_Click(
            object sender,
            RoutedEventArgs e)
        {
            NavegarA(new HorariosView());
        }

        private void NavegarA(UserControl vista)
        {
            ContentControl? contentArea = BuscarContentArea();

            if (contentArea == null)
                return;

            contentArea.Content = vista;
        }

        private ContentControl? BuscarContentArea()
        {
            DependencyObject? actual = this;

            while (actual != null)
            {
                if (actual is ContentControl content &&
                    content.Name == "ContentArea")
                    return content;

                actual =
                    System.Windows.Media.VisualTreeHelper.GetParent(actual);
            }

            return null;
        }

        /// <summary>
        /// Modelo interno usado únicamente para identificar
        /// las celdas destino durante drag and drop.
        /// </summary>
        private class DatosCeldaHorario
        {
            public string Dia { get; }

            public string RangoHora { get; }

            public DatosCeldaHorario(
                string dia,
                string rangoHora)
            {
                Dia = dia;
                RangoHora = rangoHora;
            }
        }
    }
}