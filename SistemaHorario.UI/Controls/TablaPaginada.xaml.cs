using SistemaHorario.UI.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SistemaHorario.UI.Controls
{
    /// <summary>
    /// Tabla reutilizable con paginación y acciones dinámicas.
    /// </summary>
    public partial class TablaPaginada : UserControl
    {
        private readonly List<object> _datos = new();
        private readonly List<TableColumnDefinition> _columnas = new();
        private readonly List<TableActionDefinition> _acciones = new();

        private int _paginaActual = 1;
        private int _tamanioPagina = 10;

        public event EventHandler<TableActionEventArgs>? AccionEjecutada;

        public TablaPaginada()
        {
            InitializeComponent();

            ActualizarEstado();
        }

        /// <summary>
        /// Configura columnas dinámicas.
        /// </summary>
        public void ConfigurarColumnas(List<TableColumnDefinition> columnas)
        {
            _columnas.Clear();
            _columnas.AddRange(columnas);

            ConstruirColumnas();
        }

        /// <summary>
        /// Configura acciones por fila.
        /// </summary>
        public void ConfigurarAcciones(List<TableActionDefinition> acciones)
        {
            _acciones.Clear();
            _acciones.AddRange(acciones);

            ConstruirColumnas();
        }

        /// <summary>
        /// Carga datos dinámicos.
        /// </summary>
        public void CargarDatos<T>(List<T> datos)
        {
            _datos.Clear();
            _datos.AddRange(datos.Cast<object>());

            _paginaActual = 1;

            RefrescarTabla();
        }

        /// <summary>
        /// Construye columnas del DataGrid.
        /// </summary>
        private void ConstruirColumnas()
        {
            DgDatos.Columns.Clear();

            foreach (TableColumnDefinition columna in _columnas)
            {
                // COLUMNA ESTADO
                if (columna.Binding.ToLower().Contains("estado"))
                {
                    DgDatos.Columns.Add(CrearColumnaEstado(columna));
                    continue;
                }

                DgDatos.Columns.Add(new DataGridTextColumn
                {
                    Header = columna.Header,
                    Binding = new Binding(columna.Binding),
                    Width = new DataGridLength(columna.Width, DataGridLengthUnitType.Star)
                });
            }

            if (_acciones.Count > 0)
            {
                DgDatos.Columns.Add(CrearColumnaAcciones());
            }
        }

        /// <summary>
        /// Crea columna visual para estados.
        /// </summary>
        private DataGridTemplateColumn CrearColumnaEstado(TableColumnDefinition columna)
        {
            FrameworkElementFactory border = new(typeof(Border));

            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
            border.SetValue(Border.PaddingProperty, new Thickness(3, 3, 5, 3));
            border.SetValue(Border.MinWidthProperty, 85.0);
            border.SetValue(Border.HorizontalAlignmentProperty, HorizontalAlignment.Center);

            Binding backgroundBinding = new(columna.Binding)
            {
                Converter = new EstadoBackgroundConverter()
            };

            border.SetBinding(Border.BackgroundProperty, backgroundBinding);

            FrameworkElementFactory texto = new(typeof(TextBlock));

            texto.SetBinding(TextBlock.TextProperty, new Binding(columna.Binding));

            Binding foregroundBinding = new(columna.Binding)
            {
                Converter = new EstadoForegroundConverter()
            };

            texto.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);

            texto.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            texto.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);

            border.AppendChild(texto);

            DataTemplate template = new()
            {
                VisualTree = border
            };

            return new DataGridTemplateColumn
            {
                Header = columna.Header,
                CellTemplate = template,
                Width = new DataGridLength(columna.Width, DataGridLengthUnitType.Star)
            };
        }

        /// <summary>
        /// Crea columna de acciones.
        /// </summary>
        private DataGridTemplateColumn CrearColumnaAcciones()
        {
            DataTemplate template = new();

            FrameworkElementFactory panel = new(typeof(StackPanel));

            panel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            panel.SetValue(StackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Center);

            foreach (TableActionDefinition accion in _acciones)
            {
                FrameworkElementFactory boton = new(typeof(Button));

                boton.SetValue(Button.ContentProperty, accion.Texto);
                boton.SetValue(Button.TagProperty, accion.Nombre);
                boton.SetValue(Button.WidthProperty, 36.0);
                boton.SetValue(Button.HeightProperty, 32.0);
                boton.SetValue(Button.MarginProperty, new Thickness(4, 0, 4, 0));
                boton.SetValue(Button.BorderThicknessProperty, new Thickness(0));
                boton.SetValue(Button.ForegroundProperty, Brushes.Black);
                boton.SetValue(Button.CursorProperty, System.Windows.Input.Cursors.Hand);

                // COLORES BOTONES
                switch (accion.Nombre.ToLower())
                {
                    case "editar":
                        boton.SetValue(Button.BackgroundProperty,
                            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#73AFE0")));
                        break;

                    case "eliminar":
                        boton.SetValue(Button.BackgroundProperty,
                            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EBB6B6")));
                        break;

                    case "ver":
                        boton.SetValue(Button.BackgroundProperty,
                            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDCE73")));
                        break;

                    default:
                        boton.SetValue(Button.BackgroundProperty,
                            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E7EB")));
                        break;
                }

                boton.AddHandler(Button.ClickEvent,
                    new RoutedEventHandler(BtnAccion_Click));

                panel.AppendChild(boton);
            }

            template.VisualTree = panel;

            return new DataGridTemplateColumn
            {
                Header = "Acciones",
                CellTemplate = template,
                Width = new DataGridLength(180)
            };
        }

        private void BtnAccion_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button boton)
                return;

            if (boton.Tag is not string accion)
                return;

            if (boton.DataContext == null)
                return;

            AccionEjecutada?.Invoke(
                this,
                new TableActionEventArgs(accion, boton.DataContext)
            );
        }

        private void RefrescarTabla()
        {
            int totalPaginas = ObtenerTotalPaginas();

            List<object> datosPagina = _datos
                .Skip((_paginaActual - 1) * _tamanioPagina)
                .Take(_tamanioPagina)
                .ToList();

            DgDatos.ItemsSource = datosPagina;

            TxtPagina.Text = $"Página {_paginaActual} de {totalPaginas}";

            BtnAnterior.IsEnabled = _paginaActual > 1;
            BtnSiguiente.IsEnabled = _paginaActual < totalPaginas;

            ActualizarEstado();
        }

        private int ObtenerTotalPaginas()
        {
            if (_datos.Count == 0)
                return 1;

            return (int)Math.Ceiling(_datos.Count / (double)_tamanioPagina);
        }

        private void ActualizarEstado()
        {
            bool hayDatos = _datos.Count > 0;

            DgDatos.Visibility = hayDatos
                ? Visibility.Visible
                : Visibility.Collapsed;

            TxtSinDatos.Visibility = hayDatos
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void BtnAnterior_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaActual <= 1)
                return;

            _paginaActual--;

            RefrescarTabla();
        }

        private void BtnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaActual >= ObtenerTotalPaginas())
                return;

            _paginaActual++;

            RefrescarTabla();
        }
    }

    /// <summary>
    /// Evento de acción ejecutada.
    /// </summary>
    public class TableActionEventArgs : EventArgs
    {
        public string Accion { get; }
        public object Fila { get; }

        public TableActionEventArgs(string accion, object fila)
        {
            Accion = accion;
            Fila = fila;
        }
    }

    /// <summary>
    /// Convierte estado a color de fondo.
    /// </summary>
    public class EstadoBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string estado = value?.ToString()?.ToLower() ?? "";

            return estado.Contains("act")
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B9EFC2"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FED227"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Convierte estado a color de texto.
    /// </summary>
    public class EstadoForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string estado = value?.ToString()?.ToLower() ?? "";

            return estado.Contains("act")
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A9E3B"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D6AB05"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}